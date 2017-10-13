using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RVMUtility.Data;
using System.IO;

namespace RVMUtility.Handler
{
    class RVMParser
    {
        private string[] siteNames = {
"/P502Z","/P501D","/MPROC","/E140D","/E150D","/E160D","/E245D","/E255D","/E265D","/E325D",
"/E320D","/E330D","/E423D","/E433D","/E443D","/E453D","/E473D","/E463D","/E410D","/E470D",
"/E435D","/E415D","/A245D","/A255D","/A265D","/HQ00Z","/H140D","/H150D","/H160D","/MQ00Z",
"/A320D","/A325D","/A330D","/FQ00Z","/H320D","/H325D","/H330D","/PQ00Z","/A140D","/A150D",
"/A160D","/SQ00Z","/H245D","/H255D","/H265D","/M140D","/M150D","/M160D","/M245D","/M255D",
"/M265D","/M320D","/M325D","/M330D","/M423D","/M433D","/M443D","/M453D","/M463D","/M473D",
"/M410D","/M415D","/M435D","/M470D","/P435D","/P470D","/S435D","/S470D","/X470D","/P140D",
"/P150D","/P160D","/P245D","/P255D","/P265D","/P325D","/P320D","/P330D","/P423D","/P433D",
"/P443D","/P453D","/P463D","/P473D","/P410D","/P415D","/S140D","/S150D","/S160D","/S245D",
"/S255D","/S265D","/S325D","/S320D","/S330D","/S423D","/S433D","/S443D","/S453D","/S463D",
"/S473D","/S410D","/S415D","/PA01M","/PA02M","/PA03M","/PA04M","/PA01P","/PA02P","/PA03P",
"/PA04P","/PCOT1","/PCOT2","/PCOT3","/PCOT4","/PCOT5","/PCOT6","/PCOT7","/PF01Z","/PF02Z",
"/PF03Z","/PF04Z","/PF05Z","/PWBT1","/PWBT2","/PWBT3","/PWBT4","/PWBT5","/PWBT6","/PWBT7",
"/E480D","/AA01M","/AA02M","/AA03M","/AA04M","/AF01Z","/AF02Z","/AF03Z","/AF04Z","/AF05Z",
"/AQ00Z","/E503Z","/E501Z","/E502Z","/EA01M","/EA02M","/EA03M","/EA04M","/ECOT1","/ECOT4",
"/ECOT5","/EF01Z","/EF02Z","/EF03Z","/EF04Z","/EF05Z","/EWBT1","/EWBT2","/EWBT3","/EWBT4",
"/EWBT5","/EWBT6","/EWBT7","/H503Z","/H501Z","/HA01M","/HA02M","/HA03M","/HA04M","/HF01Z",
"/HF02Z","/HF03Z","/HF04Z","/HF05Z","/MA01M","/MA02M","/MA03M","/MA04M","/MA01P","/MA03P",
"/MA04P","/MF01Z","/MF02Z","/MF03Z","/MF04Z","/MF05Z","/F503Z","/F501Z","/F502Z","/FA01M",
"/FA02M","/FA03M","/FA04M","/FA01P","/FA02P","/FA03P","/FA04P","/FCOT1","/FCOT2","/FCOT3",
"/FCOT4","/FCOT5","/FCOT6","/FCOT7","/FF01Z","/FF02Z","/FF03Z","/FF04Z","/FF05Z","/FWBT1",
"/FWBT2","/FWBT3","/FWBT4","/FWBT5","/FWBT6","/FWBT7","/FWBT8","/SF01Z","/SA01M","/S502Z",
"/SA02M","/SA03M","/SA04M","/SA02P","/SA04P","/SCOT1","/SCOT2","/SCOT3","/SCOT4","/SCOT5",
"/SCOT6","/SCOT7","/SF02Z","/SF03Z","/SF04Z","/SF05Z","/SWBT1","/SWBT2","/SWBT3","/SWBT4",
"/SWBT5","/SWBT6","/SWBT7","/M480D","/P480D","/Z503Z","/S480D",};


        public List<CNT> rootList { get; set; }
        public List<List<CNT>> allCNTList { get; set; }

        private int linelimit = 100;


        public RVMParser()
        {
            this.rootList = new List<CNT>();
            this.allCNTList = new List<List<CNT>>();
        }
        public void Prepare()
        {
            this.rootList.Clear();
            this.allCNTList.Clear();
        }

        public void Parse(object args)
        {
            string path = (string)args;


            //내용 임시저장
            Queue<string> lines = new Queue<string>();

            Stack<CNT> stackCNT = new Stack<CNT>();
            List<CNT> CntList = new List<CNT>();

            FileStream fs = new FileStream(path, FileMode.OpenOrCreate, FileAccess.Read, FileShare.ReadWrite);
            StreamReader reader = new StreamReader(fs);

            string strLine = string.Empty;
            string siteName = string.Empty;

            CNT currentCNT = null;
            bool isEnd = false;
            int primCnt = 0;
            while (reader.EndOfStream == false)
            {
                strLine = ReadLine(reader, lines);

                if (strLine == "CNTB")
                {
                    //시작 체크
                    isEnd = false;

                    strLine = ReadLine(reader, lines, 2);

                    //이름으로 생성
                    CNT newCNT = new CNT(strLine);

                    //root 표시 
                    if (this.CheckSiteName(strLine)) // 파일 이름이면.
                    {
                        this.rootList.Add(newCNT);
                        CntList = new List<CNT>();
                        this.allCNTList.Add(CntList);
                    }

                    //xyz좌표가져오기
                    strLine = ReadLine(reader, lines, 1);
                    newCNT.AddXYZ(strLine);

                    //스택에 추가
                    stackCNT.Push(newCNT);
                    CntList.Add(newCNT);

                    //현재 노드 갱신
                    if (currentCNT != null && isEnd == false)
                    {
                        currentCNT.AddCNT(newCNT);
                    }
                    currentCNT = newCNT;
                }
                else if (strLine == "CNTE")
                {
                    //그룹 끝 체크
                    isEnd = true;

                    //부모 노드로 교체
                    try
                    {
                        currentCNT = stackCNT.Pop().Parent;
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine(ex.StackTrace);
                    }
                }
                else if (strLine == "PRIM")
                {
                    primCnt++;
                    //타입 읽기
                    strLine = ReadLine(reader, lines, 2);
                    PRIM newPRIM = new PRIM(strLine);

                    //prim 추가
                    currentCNT.AddPRIM(newPRIM);


                    //회전
                    string[] rotationArr = new string[3];
                    for (int i = 0; i < 3; i++)
                    {
                        rotationArr[i] = ReadLine(reader, lines, 1);
                    }
                    newPRIM.SetRotation(rotationArr);

                    //바운드
                    string[] boundArr = new string[2];
                    for (int i = 0; i < 2; i++)
                    {
                        boundArr[i] = ReadLine(reader, lines, 1);
                    }
                    newPRIM.SetBound(boundArr);


                    if (newPRIM.Type == 7 || newPRIM.Type == 1)
                    {
                        //길이 7번( 콘은) 2줄
                        string[] lengthArr = new string[2];
                        for (int i = 0; i < 2; i++)
                        {
                            lengthArr[i] = ReadLine(reader, lines, 1);
                        }
                        newPRIM.SetLength(lengthArr);
                    }
                    else if (newPRIM.Type == 11)
                    {
                        //strLine = ReadLine(reader, lines, 6);

                        strLine = ReadLine(reader, lines, 1);
                        int faceCnt = -1;
                        int.TryParse(strLine, out faceCnt);


                        int subfaceCnt = -1;
                        for (int fi = 0; fi < faceCnt; fi++)
                        {
                            subfaceCnt = -1;
                            strLine = ReadLine(reader, lines, 1);
                            int.TryParse(strLine, out subfaceCnt); //보통 1

                            for (int sfi = 0; sfi < subfaceCnt; sfi++)
                            {
                                int vertaxCnt = -1;
                                strLine = ReadLine(reader, lines, 1);
                                int.TryParse(strLine, out vertaxCnt);

                                newPRIM.AddVertextCount(vertaxCnt);
                                Vertices newVertices = new Vertices();
                                for (int vi = 0; vi < vertaxCnt; vi++)
                                {
                                    strLine = ReadLine(reader, lines, 1);
                                    string v = strLine;
                                    strLine = ReadLine(reader, lines, 1);
                                    string r = strLine;

                                    newVertices.AddVertex(v, r);
                                }
                                newPRIM.AddVertices(newVertices);
                            }
                        }
                    }
                    else
                    {
                        //길이 1~8, 9번 1줄
                        string[] lengthArr = new string[1];
                        lengthArr[0] = ReadLine(reader, lines, 1);
                        newPRIM.SetLength(lengthArr);
                    }
                }
            }
        }

        private string ReadLine(StreamReader reader, Queue<string> lines, int number = 1)
        {
            string strLine = string.Empty;

            for (int i = 0; i < number; i++)
            {
                strLine = reader.ReadLine();
                lines.Enqueue(strLine);
                if (lines.Count > linelimit) lines.Dequeue();
            }
            return strLine;
        }

        private bool CheckSiteName(string str)
        {
            bool result = false;
            for (int i = 0; i < this.siteNames.Length; i++)
            {
                if (this.siteNames[i] == str)
                {
                    result = true;
                    break;
                }
            }
            return result;
        }


        RVMToFBXConverter converter = null;
        public void StartConvert()
        {
            converter = new RVMToFBXConverter();
            converter.Prepare(this.rootList);
            converter.Convert();
        }
    }
}
