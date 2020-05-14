using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;


namespace calculator
{
    class RPN_calculator
    {
        static void Main(string[] args)
        {
            // variable to store entered string
            string str = "";

            if (args.Length == 0) { 
                Console.WriteLine("Greatings from the Calculator program!\n");
                Console.WriteLine("Please enter an algebraic statement or first number");
                Console.WriteLine("1. you can use positive and negative rational numbers, brackets and such operators as addition, subtraction, multiplication or division.");
                Console.WriteLine("2. you can run program passing algorithmic statement as a command line argument.\n");

                while(str == "")
                    str = Console.ReadLine();

                if (IsStatement(str))
                {
                    Console.WriteLine("Answer: " + Statement(str));
                }
                else
                {
                    Number(Double.Parse(str));
                }
            }
            else
            {
                // run the program with command line arguments
                Console.WriteLine(Statement(args[0]));
            }
        }


        private static void Number(double number1)
        {
            double number2;
            string sign;
            Console.WriteLine("Please enter second number.");

            number2 = GetNumber();

            Console.WriteLine("Please enter a sing.");
            while (true)
            {
                sign = Console.ReadLine();
                if (IsSign(sign))
                    break;
                Console.WriteLine("Plese enter a specific sign to generating algebraic statement (one of: '+', '-', '*' or '/')");
            }

            double answer = Calculation(number1, number2, sign);
            Console.WriteLine("Entered statement: " + number1 + " " + sign + " " + number2);
            Console.WriteLine("Answer: " + answer);
        }


        private static double Calculation(double number1, double number2, string sign)
        {
            double answer = Double.MinValue;
            switch (sign)
            {
                case "+":
                    answer = number1 + number2;
                    break;
                case "-":
                    answer = number1 - number2;
                    break;
                case "*":
                    answer = number1 * number2;
                    break;
                case "/":
                    answer = number1 / number2;
                    break;
            }
            return answer;
        }


        private static bool IsSign(string sign)
        {
            return (sign.Equals("-") || sign.Equals("+") || sign.Equals("*") || sign.Equals("/"));
        }


        private static double GetNumber()
        {
            double number;
            while (true)
            {
                try
                {
                    number = Double.Parse(Console.ReadLine());
                }
                catch (Exception)
                {
                    Console.WriteLine("Plese enter a valid natural number to complite algebraic statement.");
                    continue;
                }
                return number;
            }
        }


        private static double Statement(string str)
        {
            double answer = Double.MinValue;
            bool done = false;
            while (!done)
            {
                try
                {
                    if (str.Contains(@"[\w]") || !(str.Contains('+') || str.Contains('-')
                        || str.Contains('*') || str.Contains('/')))
                        throw new ArithmeticException();

                    List<string> stackRPN = GetReversePolishNotation(str);

                    // printing stack of Reverse Polish notation
                    //foreach (string each in stackRPN)
                    //    Console.Write(each);
                    //Console.WriteLine();

                    answer = CalculateReversePolishNotation(stackRPN);
                    done = true;
                }
                catch (Exception)
                {
                    Console.WriteLine("Input error. Enter a valid algoritmic statement.");
                    str = Console.ReadLine();
                }
            }
            return answer;
        }


        private static double CalculateReversePolishNotation(List<string> stackRPN)
        {
            double answer = Double.MinValue;

            int i1;
            int i2;

            int i = 0;

            while(i < stackRPN.Count)
            {
                if (IsSign(stackRPN[i]))
                {
                    i2 = i - 1;
                    while (stackRPN[i2].Equals("") || IsSign(stackRPN[i2]))
                        i2--;
                    i1 = i2 - 1;
                    while (stackRPN[i1].Equals("") || IsSign(stackRPN[i1]))
                        i1--;

                    answer = Calculation(Double.Parse(stackRPN[i1]), Double.Parse(stackRPN[i2]), stackRPN[i]);

                    stackRPN[i1] = "";
                    stackRPN[i2] = answer.ToString();
                    stackRPN[i] = "";
                }
                i++;
            }

            // checking if RPN calculates properly
            int count = 0;
            for (int n = 0; n < stackRPN.Count; n++)
                if (!stackRPN[n].Equals(""))
                    count++;
            if (count != 1)
                throw new ArithmeticException();

            return answer;
        }


        static private List <String> GetReversePolishNotation(string str)
        {
            str = str.Trim();
            int i = 0;
            int j = 0;
            List<string> result = new List<string>();
            List<string> stack = new List<string>();

            while (i < str.Length)
            {
                while (str[i] == ' ')
                    i++;
                if (str[i] >= '0' && str[i] <= '9' ||
                    (i == 0 && str[i] == '-' && str[i + 1] >= '0' && str[i] <= '9') ||
                    (i > 0 && i < str.Length - 1 && (IsSign(Char.ToString(str[i - 1])) ||
                    str[i - 1] == '(') && str[i] == '-' && str[i + 1] >= '0' && str[i + 1] <= '9'))
                {
                    j = i;
                    if (str[i] == '-')
                        i++;
                    while (i < str.Length && str[i] >= '0' && str[i] <= '9')
                        i++;
                    if (i < str.Length && str[i] == '.')
                        i++;
                    while (i < str.Length && str[i] >= '0' && str[i] <= '9' && i < str.Length)
                        i++;
                    result.Add(str.Substring(j, i - j));
                    i--;
                }
                else
                if (str[i] == '+' || str[i] == '-' || str[i] == '*' || str[i] == '/')
                {
                    if ((str[i] == '+' || str[i] == '-') && stack.Count != 0)
                        if (stack[^1].Equals("*") || stack[^1].Equals("/") ||
                        stack[^1].Equals("+") || stack[^1].Equals("-"))
                        {
                            result.Add(stack[^1]);
                            stack.RemoveAt(stack.Count - 1);
                        }
                    stack.Add(Char.ToString(str[i]));
                }
                else
                    if (str[i] == '(')
                    stack.Add(Char.ToString(str[i]));
                else
                    if (str[i] == ')')
                {
                    j = stack.Count - 1;
                    while (!stack[j].Equals("(") && j >= 0)
                    {
                        result.Add(stack[j]);
                        stack.RemoveAt(j);
                        j--;
                    }
                    stack.RemoveAt(j);
                }
                i++;
            }
            j = stack.Count - 1;
            while (j >= 0)
            {
                result.Add(stack[j]);
                j--;
            }
            return result;
        }


        private static void IsDigit(string str) 
        {
            throw new NotImplementedException();
        }


        private static bool IsStatement(string str)
        {
            string patternNumber = @"(\d+\.?\d*)";
            Match mNumber = Regex.Match(str, patternNumber, RegexOptions.None);
            mNumber = mNumber.NextMatch();
            return !mNumber.Value.Equals("");
        }
    }
}
