namespace JX3CalculatorShared.Utils
{
    public static class ReflectionTool
    {
        public static bool HasField(this object obj, string name)
        {
            var field = obj.GetType().GetField(name);
            bool res = field != null;
            return res;
        }

        public static object GetField(this object obj, string name)
        {
            var field = obj.GetType().GetField(name);
            var res = field.GetValue(obj);
            return res;
        }

        public static void SetField(this object obj, string name, object value)
        {
            var field = obj.GetType().GetField(name);
            field.SetValue(obj, value);
        }

        public static bool HasProperty(this object obj, string name)
        {
            var property = obj.GetType().GetProperty(name);
            bool res = property != null;
            return res;
        }

        public static object GetProperty(this object obj, string name)
        {
            var field = obj.GetType().GetProperty(name);
            var res = field.GetValue(obj);
            return res;
        }
    }
}