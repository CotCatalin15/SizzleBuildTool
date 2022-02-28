using SizzleBuildTool.Commands.Arguments;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace SizzleBuildTool.Commands
{
    class FieldEntry
    {
        string _type;
        string _name;
        List<string> _properties;
        List<KeyValuePair<string, object>> _expressions;

        public FieldEntry(string Type, string Name, List<string> Properties, 
            List<KeyValuePair<string, object>> ExpresionProperties)
        {
            _type = Type;
            _name = Name;
            _properties = Properties;

            //Expressions are not supported, yet
            //_expressions = ExpresionProperties;
        }

        public string Type => _type;
        public string Name => _name;
        public List<string> Properties => _properties;
        public List<KeyValuePair<string, object>> Expression => _expressions;
    }

    class ParseClass : IBuildCommand
    {
        string _className;
        string[] _subclasses;

        List<FieldEntry> _fieldEntries;

        public void AddField(FieldEntry entry)
        {
            _fieldEntries.Add(entry);
        }

        public bool CanExecute(ICommandArgument[] arguments)
        {
            return true;
        }

        public void Execute(FileParser Parser, ICommandArgument[] arguments)
        {
            _fieldEntries = new List<FieldEntry>();

            string Line = Parser.ReadNextLine(this);

            if(Line == null)
            {
                return;
            }

            //this line must contained class
            if (!Line.Contains("class"))
            {
                throw new Exception("Next line after ParseClass must be the class");
            }
            string ClassLine = Line;

            if (!Parser.ReadNextLine(this).Contains("{"))
            {
                throw new Exception("Must be a full class(parser did not find '{')");
            }

            ClassLine = ClassLine.Replace("class ", "");
            ClassLine = ClassLine.Replace("public ", "");
            ClassLine = ClassLine.Replace("private ", "");
            ClassLine = ClassLine.Replace("protected ", "");
            if(arguments.Length > 0)
            ClassLine = ClassLine.Replace(arguments[0].Value as string, "");

            string[] ClassLineSplit = ClassLine.Split(new char[] { ' ', ':' }, StringSplitOptions.RemoveEmptyEntries);

            _className = ClassLineSplit[0];

            if (ClassLineSplit.Length > 1)
            {
                _subclasses = new string[ClassLineSplit.Length - 1];
                for (int i = 0; i < _subclasses.Length; ++i)
                {
                    _subclasses[i] = ClassLineSplit[i + 1];
                }
            }

            while (Line != null && !Line.Contains("};"))
            {
                Line = Parser.ReadNextLine(this);
            }

            //Build SClass
            BuildSClass(Parser, arguments);
        }

        static string SClassGenString = "";

        object lockObj = new object();

        static public void LoadSGenClass()
        {
            StreamReader reader = new StreamReader("SClassGenerator.gen");
            SClassGenString = reader.ReadToEnd();
            reader.Close();

        }

        private void BuildSClass(FileParser Parser, ICommandArgument[] arguments)
        {
            lock (lockObj)
            {
                if (SClassGenString.Length == 0)
                {
                    LoadSGenClass();
                }
            }

            string GeneratedClass = SClassGenString.Replace("${ClassName}", _className);
            GeneratedClass = GeneratedClass.Replace("${SClassName}", $"{_className}_SClass");
            GeneratedClass = GeneratedClass.Replace("${DerivedClassName}", _subclasses[0]);
            GeneratedClass = GeneratedClass.Replace("${ClassFields}", CreateSFieldString(_className));

            string api = "";
            foreach (var arg in arguments)
            {
                if (arg is ExpressionCommandArgument expr)
                {
                    if (expr.VarName == "API")
                    {
                        api = expr.Value as string;
                        if (api == "")
                            Console.WriteLine("Error: API = <string>");
                            break;
                    }
                }
            }

            GeneratedClass = GeneratedClass.Replace("${ClassAPI}", api);


            Parser.WriteToOutput(GeneratedClass);
        }


        private string CreateSFieldString(string ClassName)
        {
            StringBuilder sb = new StringBuilder();

            string ReplaceString = "_fields.emplace_back( SField::CreateField<{0}>(" +
                "\"{0}\", \"{1}\", sizeof({0}), offsetof({2}, {1}), {3}" +
                ") );\\";

            for (int i = 0; i < _fieldEntries.Count; ++i)
            {
                var field = _fieldEntries[i];

                StringBuilder PropBuilder = new StringBuilder();

                PropBuilder.Append("{");
                for(int j = 0; j < field.Properties.Count; ++j)
                {
                    PropBuilder.Append("\"" + field.Properties[j] + "\"");
                    if (i != field.Properties.Count - 1)
                        PropBuilder.Append(",");
                }

                PropBuilder.Append("}");

                string PropertyString = PropBuilder.ToString();

                PropBuilder.Clear();

               

                string NewString = String.Format(ReplaceString,
                    field.Type, field.Name, ClassName, PropertyString);
                sb.Append(NewString);

                if (i != _fieldEntries.Count - 1)
                    sb.Append('\n');
            }

            if(_fieldEntries.Count == 0)
            {
                return @"\";    
            }

            return sb.ToString();
        }

        public string BuildName => "ParseClass";

    }
}
