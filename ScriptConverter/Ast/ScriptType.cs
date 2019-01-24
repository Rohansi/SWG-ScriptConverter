using System.Linq;

namespace ScriptConverter.Ast
{
    class ScriptType
    {
        public string Name { get; private set; }
        public bool IsArray { get { return ArrayDimensions > 0; } }
        public bool IsResizable { get; private set; }
        public int ArrayDimensions { get; private set; }

        public ScriptType(string name, int arrayDimensions = 0, bool isResizable = false)
        {
            if (name == "unknown")
                name = "Object";

            if (name == "string")
                name = "String";

            if (name == "modifyable_int")
                name = "modifiable_int";

            if (name == "modifyable _float")
                name = "modifiable_float";

            if (name == "modifyable_string_id")
                name = "modifiable_string_id";

            if (isResizable)
                arrayDimensions = 1;

            Name = name;
            IsResizable = isResizable;
            ArrayDimensions = arrayDimensions;
        }

        public override string ToString()
        {
            if (IsResizable)
                return "Vector";

            if (!IsArray)
                return Name;

            return Name + string.Join("", Enumerable.Repeat("[]", ArrayDimensions));
        }
    }
}
