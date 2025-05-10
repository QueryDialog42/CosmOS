using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Controls;

namespace WPFFrameworkApp2
{
    public partial class CalculatorApp : Window
    {
        private string _expression = "";

        public CalculatorApp()
        {
            InitializeComponent();
            Show();
        }

        private void Digit_Click(object sender, RoutedEventArgs e)
        {
            var d = ((Button)sender).Content.ToString();
            if (_expression == "0")
                _expression = d;
            else
                _expression += d;
            Display.Text = _expression;
        }


        private void Decimal_Click(object sender, RoutedEventArgs e)
        {

            if (string.IsNullOrEmpty(_expression) || Regex.IsMatch(_expression[^1].ToString(), @"[\+\-\*/]"))
            {
                _expression += "0,";
            }
            else
            {

                var parts = Regex.Split(_expression, @"[\+\-\*/]");
                var lastNumber = parts[^1];
                if (lastNumber.Contains(","))
                    return;
                _expression += ",";
            }
            Display.Text = _expression;
        }


        private void Operator_Click(object sender, RoutedEventArgs e)
        {
            var op = ((Button)sender).Content.ToString();
            if (string.IsNullOrEmpty(_expression))
                return;


            if (Regex.IsMatch(_expression[^1].ToString(), @"[\+\-\*/]"))
                _expression = _expression[..^1] + op;
            else
                _expression += op;

            Display.Text = _expression;
        }


        private void Equal_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrEmpty(_expression))
                return;

            if (Regex.IsMatch(_expression[^1].ToString(), @"[\+\-\*/]"))
                _expression = _expression[..^1];

            try
            {

                var standardized = _expression.Replace(',', '.');
                double result = EvaluateExpression(standardized);

                _expression = result.ToString(CultureInfo.CurrentCulture);
                Display.Text = _expression;
            }
            catch (DivideByZeroException)
            {
                Display.Text = "Invalid";
                _expression = "";
            }
            catch (Exception)
            {
                MessageBox.Show("Invalid operation!", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                ClearAll();
            }
        }


        private void Clear_Click(object sender, RoutedEventArgs e) => ClearAll();
        private void ClearAll()
        {
            _expression = "";
            Display.Text = "0";
        }


        private double EvaluateExpression(string expr)
        {

            var numberPattern = @"-?\d+(\.\d+)?";
            var opPattern = @"[\+\-\*/]";
            var tokenPattern = numberPattern + "|" + opPattern;

            var tokens = new List<string>();
            foreach (Match m in Regex.Matches(expr, tokenPattern))
                tokens.Add(m.Value);

            if (tokens.Count == 0)
                throw new Exception("No token");


            for (int i = 1; i < tokens.Count; i += 2)
            {
                string op = tokens[i];
                if (op == "*" || op == "/")
                {
                    double left = double.Parse(tokens[i - 1], CultureInfo.InvariantCulture);
                    double right = double.Parse(tokens[i + 1], CultureInfo.InvariantCulture);
                    double result = Compute(left, op, right);


                    tokens[i - 1] = result.ToString(CultureInfo.InvariantCulture);
                    tokens.RemoveRange(i, 2);
                    i -= 2;
                }
            }

            double finalResult = double.Parse(tokens[0], CultureInfo.InvariantCulture);
            for (int i = 1; i < tokens.Count; i += 2)
            {
                string op = tokens[i];
                double right = double.Parse(tokens[i + 1], CultureInfo.InvariantCulture);
                finalResult = Compute(finalResult, op, right);
            }

            return finalResult;
        }


        private double Compute(double left, string op, double right)
        {
            return op switch
            {
                "+" => left + right,
                "-" => left - right,
                "*" => left * right,
                "/" => right == 0
                          ? throw new DivideByZeroException()
                          : left / right,
                _ => throw new InvalidOperationException()
            };
        }
    }
}