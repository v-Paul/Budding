using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.InteropServices;
using System.Threading;
using System.Collections;

namespace BaseSturct
{

    public class Operation
    {
        private Stack<string> staData;
        private List<string> lstPubOperate;

        public Operation(scriptSig inputSig, string outputScript)
        {
            this.staData = new Stack<string>();
            this.AddData(inputSig.Signature);
            this.AddData(inputSig.PubKey);

            this.lstPubOperate = new List<string>();
            this.lstPubOperate = this.PubScri2list(outputScript);

        }
        public Operation()
        {
            this.staData = new Stack<string>();
            this.lstPubOperate = new List<string>();

        }

        public List<string> PubScri2list(string strscriptPubKey)
        {
            string[] sArray = strscriptPubKey.Split(' ');
            List<string> lstTemp = new List<string>(sArray);
            return lstTemp;
        }

        public void AddData(string strData)
        {
            staData.Push(strData);
        }

        private void OP_DUP()
        {
            string strTemp = this.staData.Peek();
            this.staData.Push(strTemp);
        }

        private void OP_HASH160()
        {
            string strTemp = this.staData.Pop();
            strTemp = Cryptor.SHA256(strTemp, strTemp.Length);
            this.staData.Push(strTemp);

        }

        private bool OP_EQUALVERIFY()
        {
            string strCurrentHash = this.staData.Pop();
            string strOrigHash = this.staData.Pop();

            if (string.Equals(strCurrentHash, strOrigHash))
            {
                return true;
            }
            else
                return false;
        }
        string OP_CHECKSIG()
        {
            string strPubkey = this.staData.Pop();
            string strSign = this.staData.Pop();

            string strRet = Cryptor.rsaPubDecrySign(strSign, strPubkey, false);
            return strRet;
        }

        public bool RunScript(ref string strOutTxt)
        {
            strOutTxt = string.Empty;
            foreach(string str in this.lstPubOperate)
            {
                if(str.Substring(0,3) == "OP_")
                {
                    switch (str)
                    {//OP_DUP OP_HASH160 ab68025513c3dbd2f7b92a94e0581f5d50f654e7 OP_EQUALVERIFY OP_CHECKSIG"          
                        case "OP_DUP":
                            this.OP_DUP();
                            break;
                        case "OP_HASH160":
                            this.OP_HASH160();
                            break;
                        case "OP_EQUALVERIFY":
                            if (!this.OP_EQUALVERIFY())
                                return false;
                            break;
                        case "OP_CHECKSIG":
                            strOutTxt = this.OP_CHECKSIG();
                            break;
                        default:
                            break;
                    }
                }
                else
                {
                    this.AddData(str);
                }
            }

            return true;
        }

    }

    //public class Operation
    //{
    //    private static int ADDITION = 1;
    //    private static int SUBTRACTION = 1;
    //    private static int MULTIPLICATION = 2;
    //    private static int DIVISION = 2;

    //    public static int getValue(String operation)
    //    {
    //        int result;
    //        switch (operation)
    //        {
    //            case "+":
    //                result = ADDITION;
    //                break;
    //            case "-":
    //                result = SUBTRACTION;
    //                break;
    //            case "*":
    //                result = MULTIPLICATION;
    //                break;
    //            case "/":
    //                result = DIVISION;
    //                break;
    //            default:
    //                //                System.out.println("不存在该运算符");
    //                result = 0;
    //                break;
    //        }
    //        return result;
    //    }
    //}



    //public class PolishNotation
    //{
    //    /**
    //     * 把字符串转换成中序表达式
    //     * @param s
    //     * @return
    //     */
    //    public static List<String> toInfixExpression(String s)
    //    {
    //        List<String> ls = new List<String>();//存储中序表达式
    //        int i = 0;
    //        String str;
    //        char c;
    //        do
    //        {
    //            if ((c = s[i]) < 48 || (c = s[i]) > 57)
    //            {
    //                ls.Add("" + c);
    //                i++;
    //            }
    //            else
    //            {
    //                str = "";
    //                while (i < s.Length && (c = s[i]) >= 48
    //                        && (c = s[i]) <= 57)
    //                {
    //                    str += c;
    //                    i++;
    //                }
    //                ls.Add(str);
    //            }

    //        } while (i < s.Length);
    //        return ls;
    //    }

    //    /**
    //     * 转换成逆波兰表达式
    //     * @param ls
    //     * @return
    //     */
    //    public static List<String> parseSuffixExpression(List<String> ls)
    //    {
    //        Stack<String> s1 = new Stack<String>();
    //        Stack<String> s2 = new Stack<String>();
    //        List<String> lss = new List<String>();
    //        foreach (String ss in ls)
    //        {
    //            if (ss.matches("\\d+"))
    //            {
    //                lss.add(ss);
    //            }
    //            else if (ss.equals("("))
    //            {
    //                s1.push(ss);
    //            }
    //            else if (ss.equals(")"))
    //            {

    //                while (!s1.peek().equals("("))
    //                {
    //                    lss.add(s1.pop());
    //                }
    //                s1.pop();
    //            }
    //            else
    //            {
    //                while (s1.size() != 0 && Operation.getValue(s1.peek()) >= Operation.getValue(ss))
    //                {
    //                    lss.add(s1.pop());
    //                }
    //                s1.push(ss);
    //            }
    //        }
    //        while (s1.size() != 0)
    //        {
    //            lss.add(s1.pop());
    //        }
    //        return lss;
    //    }

    //    /**
    //     * 通过逆波兰表达式计算结果
    //     * @param ls
    //     * @return
    //     */
    //    public static int calculate(List<String> ls)
    //    {
    //        Stack<String> s = new Stack<String>();
    //        for (String str : ls)
    //        {
    //            if (str.matches("\\d+"))
    //            {
    //                s.push(str);
    //            }
    //            else
    //            {
    //                int b = Integer.parseInt(s.pop());
    //                int a = Integer.parseInt(s.pop());
    //                int result = 0;
    //                if (str.equals("+"))
    //                {
    //                    result = a + b;
    //                }
    //                else if (str.equals("-"))
    //                {
    //                    result = a - b;
    //                }
    //                else if (str.equals("*"))
    //                {
    //                    result = a * b;
    //                }
    //                else if (str.equals("\\"))
    //                {
    //                    result = a / b;
    //                }
    //                s.push("" + result);
    //            }
    //        }
    //        System.out.println(s.peek());
    //        return Integer.parseInt(s.pop());
    //    }
    //}
}
