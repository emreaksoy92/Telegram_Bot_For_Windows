using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections;
using System.Net;
using Telegram_Bot.SimpleJSON;
using System.Collections.Specialized;
using System.Net.Http;
using System.IO;


namespace Telegram_Bot
{

    namespace SimpleJSON
    {
        public enum JSONNodeType
        {
            Array = 1,
            Object = 2,
            String = 3,
            Number = 4,
            NullValue = 5,
            Boolean = 6,
            None = 7,
        }
        public enum JSONTextMode
        {
            Compact,
            Indent
        }

        public abstract partial class JSONNode
        {
            #region common interface

            public virtual JSONNode this[int aIndex] { get { return null; } set { } }

            public virtual JSONNode this[string aKey] { get { return null; } set { } }

            public virtual string Value { get { return ""; } set { } }

            public virtual int Count { get { return 0; } }

            public virtual bool IsNumber { get { return false; } }
            public virtual bool IsString { get { return false; } }
            public virtual bool IsBoolean { get { return false; } }
            public virtual bool IsNull { get { return false; } }
            public virtual bool IsArray { get { return false; } }
            public virtual bool IsObject { get { return false; } }

            public virtual void Add(string aKey, JSONNode aItem)
            {
            }
            public virtual void Add(JSONNode aItem)
            {
                Add("", aItem);
            }

            public virtual JSONNode Remove(string aKey)
            {
                return null;
            }

            public virtual JSONNode Remove(int aIndex)
            {
                return null;
            }

            public virtual JSONNode Remove(JSONNode aNode)
            {
                return aNode;
            }

            public virtual IEnumerable<JSONNode> Children
            {
                get
                {
                    yield break;
                }
            }

            public IEnumerable<JSONNode> DeepChildren
            {
                get
                {
                    foreach (var C in Children)
                        foreach (var D in C.DeepChildren)
                            yield return D;
                }
            }

            public override string ToString()
            {
                StringBuilder sb = new StringBuilder();
                WriteToStringBuilder(sb, 0, 0, JSONTextMode.Compact);
                return sb.ToString();
            }

            public virtual string ToString(int aIndent)
            {
                StringBuilder sb = new StringBuilder();
                WriteToStringBuilder(sb, 0, aIndent, JSONTextMode.Indent);
                return sb.ToString();
            }
            internal abstract void WriteToStringBuilder(StringBuilder aSB, int aIndent, int aIndentInc, JSONTextMode aMode);

            #endregion common interface

            #region typecasting properties

            public abstract JSONNodeType Tag { get; }

            public virtual double AsDouble
            {
                get
                {
                    double v = 0.0;
                    if (double.TryParse(Value, out v))
                        return v;
                    return 0.0;
                }
                set
                {
                    Value = value.ToString();
                }
            }

            public virtual int AsInt
            {
                get { return (int)AsDouble; }
                set { AsDouble = value; }
            }

            public virtual float AsFloat
            {
                get { return (float)AsDouble; }
                set { AsDouble = value; }
            }

            public virtual bool AsBool
            {
                get
                {
                    bool v = false;
                    if (bool.TryParse(Value, out v))
                        return v;
                    return !string.IsNullOrEmpty(Value);
                }
                set
                {
                    Value = (value) ? "true" : "false";
                }
            }

            public virtual JSONArray AsArray
            {
                get
                {
                    return this as JSONArray;
                }
            }

            public virtual JSONObject AsObject
            {
                get
                {
                    return this as JSONObject;
                }
            }


            #endregion typecasting properties

            #region operators

            public static implicit operator JSONNode(string s)
            {
                return new JSONString(s);
            }
            public static implicit operator string(JSONNode d)
            {
                return (d == null) ? null : d.Value;
            }

            public static implicit operator JSONNode(double n)
            {
                return new JSONNumber(n);
            }
            public static implicit operator double(JSONNode d)
            {
                return (d == null) ? 0 : d.AsDouble;
            }

            public static implicit operator JSONNode(float n)
            {
                return new JSONNumber(n);
            }
            public static implicit operator float(JSONNode d)
            {
                return (d == null) ? 0 : d.AsFloat;
            }

            public static implicit operator JSONNode(int n)
            {
                return new JSONNumber(n);
            }
            public static implicit operator int(JSONNode d)
            {
                return (d == null) ? 0 : d.AsInt;
            }

            public static implicit operator JSONNode(bool b)
            {
                return new JSONBool(b);
            }
            public static implicit operator bool(JSONNode d)
            {
                return (d == null) ? false : d.AsBool;
            }

            public static bool operator ==(JSONNode a, object b)
            {
                if (ReferenceEquals(a, b))
                    return true;
                bool aIsNull = a is JSONNull || ReferenceEquals(a, null) || a is JSONLazyCreator;
                bool bIsNull = b is JSONNull || ReferenceEquals(b, null) || b is JSONLazyCreator;
                if (aIsNull && bIsNull)
                    return true;
                return a.Equals(b);
            }

            public static bool operator !=(JSONNode a, object b)
            {
                return !(a == b);
            }

            public override bool Equals(object obj)
            {
                return ReferenceEquals(this, obj);
            }

            public override int GetHashCode()
            {
                return base.GetHashCode();
            }

            #endregion operators
            internal static StringBuilder m_EscapeBuilder = new StringBuilder();
            internal static string Escape(string aText)
            {
                m_EscapeBuilder.Length = 0;
                if (m_EscapeBuilder.Capacity < aText.Length + aText.Length / 10)
                    m_EscapeBuilder.Capacity = aText.Length + aText.Length / 10;
                foreach (char c in aText)
                {
                    switch (c)
                    {
                        case '\\':
                            m_EscapeBuilder.Append("\\\\");
                            break;
                        case '\"':
                            m_EscapeBuilder.Append("\\\"");
                            break;
                        case '\n':
                            m_EscapeBuilder.Append("\\n");
                            break;
                        case '\r':
                            m_EscapeBuilder.Append("\\r");
                            break;
                        case '\t':
                            m_EscapeBuilder.Append("\\t");
                            break;
                        case '\b':
                            m_EscapeBuilder.Append("\\b");
                            break;
                        case '\f':
                            m_EscapeBuilder.Append("\\f");
                            break;
                        default:
                            m_EscapeBuilder.Append(c);
                            break;
                    }
                }
                string result = m_EscapeBuilder.ToString();
                m_EscapeBuilder.Length = 0;
                return result;
            }

            static void ParseElement(JSONNode ctx, string token, string tokenName, bool quoted)
            {
                if (quoted)
                {
                    ctx.Add(tokenName, token);
                    return;
                }
                string tmp = token.ToLower();
                if (tmp == "false" || tmp == "true")
                    ctx.Add(tokenName, tmp == "true");
                else if (tmp == "null")
                    ctx.Add(tokenName, null);
                else
                {
                    double val;
                    if (double.TryParse(token, out val))
                        ctx.Add(tokenName, val);
                    else
                        ctx.Add(tokenName, token);
                }
            }

            public static JSONNode Parse(string aJSON)
            {
                Stack<JSONNode> stack = new Stack<JSONNode>();
                JSONNode ctx = null;
                int i = 0;
                StringBuilder Token = new StringBuilder();
                string TokenName = "";
                bool QuoteMode = false;
                bool TokenIsQuoted = false;
                while (i < aJSON.Length)
                {
                    switch (aJSON[i])
                    {
                        case '{':
                            if (QuoteMode)
                            {
                                Token.Append(aJSON[i]);
                                break;
                            }
                            stack.Push(new JSONObject());
                            if (ctx != null)
                            {
                                ctx.Add(TokenName, stack.Peek());
                            }
                            TokenName = "";
                            Token.Length = 0;
                            ctx = stack.Peek();
                            break;

                        case '[':
                            if (QuoteMode)
                            {
                                Token.Append(aJSON[i]);
                                break;
                            }

                            stack.Push(new JSONArray());
                            if (ctx != null)
                            {
                                ctx.Add(TokenName, stack.Peek());
                            }
                            TokenName = "";
                            Token.Length = 0;
                            ctx = stack.Peek();
                            break;

                        case '}':
                        case ']':
                            if (QuoteMode)
                            {

                                Token.Append(aJSON[i]);
                                break;
                            }
                            if (stack.Count == 0)
                                throw new Exception("JSON Parse: Too many closing brackets");

                            stack.Pop();
                            if (Token.Length > 0 || TokenIsQuoted)
                            {
                                ParseElement(ctx, Token.ToString(), TokenName, TokenIsQuoted);
                                TokenIsQuoted = false;
                            }
                            TokenName = "";
                            Token.Length = 0;
                            if (stack.Count > 0)
                                ctx = stack.Peek();
                            break;

                        case ':':
                            if (QuoteMode)
                            {
                                Token.Append(aJSON[i]);
                                break;
                            }
                            TokenName = Token.ToString();
                            Token.Length = 0;
                            TokenIsQuoted = false;
                            break;

                        case '"':
                            QuoteMode ^= true;
                            TokenIsQuoted |= QuoteMode;
                            break;

                        case ',':
                            if (QuoteMode)
                            {
                                Token.Append(aJSON[i]);
                                break;
                            }
                            if (Token.Length > 0 || TokenIsQuoted)
                            {
                                ParseElement(ctx, Token.ToString(), TokenName, TokenIsQuoted);
                                TokenIsQuoted = false;
                            }
                            TokenName = "";
                            Token.Length = 0;
                            TokenIsQuoted = false;
                            break;

                        case '\r':
                        case '\n':
                            break;

                        case ' ':
                        case '\t':
                            if (QuoteMode)
                                Token.Append(aJSON[i]);
                            break;

                        case '\\':
                            ++i;
                            if (QuoteMode)
                            {
                                char C = aJSON[i];
                                switch (C)
                                {
                                    case 't':
                                        Token.Append('\t');
                                        break;
                                    case 'r':
                                        Token.Append('\r');
                                        break;
                                    case 'n':
                                        Token.Append('\n');
                                        break;
                                    case 'b':
                                        Token.Append('\b');
                                        break;
                                    case 'f':
                                        Token.Append('\f');
                                        break;
                                    case 'u':
                                        {
                                            string s = aJSON.Substring(i + 1, 4);
                                            Token.Append((char)int.Parse(
                                                s,
                                                System.Globalization.NumberStyles.AllowHexSpecifier));
                                            i += 4;
                                            break;
                                        }
                                    default:
                                        Token.Append(C);
                                        break;
                                }
                            }
                            break;

                        default:
                            Token.Append(aJSON[i]);
                            break;
                    }
                    ++i;
                }
                if (QuoteMode)
                {
                    throw new Exception("JSON Parse: Quotation marks seems to be messed up.");
                }
                return ctx;
            }

            public virtual void Serialize(System.IO.BinaryWriter aWriter)
            {
            }

            public void SaveToStream(System.IO.Stream aData)
            {
                var W = new System.IO.BinaryWriter(aData);
                Serialize(W);
            }

#if USE_SharpZipLib
		public void SaveToCompressedStream(System.IO.Stream aData)
		{
			using (var gzipOut = new ICSharpCode.SharpZipLib.BZip2.BZip2OutputStream(aData))
			{
				gzipOut.IsStreamOwner = false;
				SaveToStream(gzipOut);
				gzipOut.Close();
			}
		}
 
		public void SaveToCompressedFile(string aFileName)
		{
 
#if USE_FileIO
			System.IO.Directory.CreateDirectory((new System.IO.FileInfo(aFileName)).Directory.FullName);
			using(var F = System.IO.File.OpenWrite(aFileName))
			{
				SaveToCompressedStream(F);
			}
 
#else
			throw new Exception("Can't use File IO stuff in the webplayer");
#endif
		}
		public string SaveToCompressedBase64()
		{
			using (var stream = new System.IO.MemoryStream())
			{
				SaveToCompressedStream(stream);
				stream.Position = 0;
				return System.Convert.ToBase64String(stream.ToArray());
			}
		}
 
#else
            public void SaveToCompressedStream(System.IO.Stream aData)
            {
                throw new Exception("Can't use compressed functions. You need include the SharpZipLib and uncomment the define at the top of SimpleJSON");
            }

            public void SaveToCompressedFile(string aFileName)
            {
                throw new Exception("Can't use compressed functions. You need include the SharpZipLib and uncomment the define at the top of SimpleJSON");
            }

            public string SaveToCompressedBase64()
            {
                throw new Exception("Can't use compressed functions. You need include the SharpZipLib and uncomment the define at the top of SimpleJSON");
            }
#endif

            public void SaveToFile(string aFileName)
            {
#if USE_FileIO
            System.IO.Directory.CreateDirectory((new System.IO.FileInfo(aFileName)).Directory.FullName);
            using (var F = System.IO.File.OpenWrite(aFileName))
            {
                SaveToStream(F);
            }
#else
                throw new Exception("Can't use File IO stuff in the webplayer");
#endif
            }

            public string SaveToBase64()
            {
                using (var stream = new System.IO.MemoryStream())
                {
                    SaveToStream(stream);
                    stream.Position = 0;
                    return System.Convert.ToBase64String(stream.ToArray());
                }
            }

            public static JSONNode Deserialize(System.IO.BinaryReader aReader)
            {
                JSONNodeType type = (JSONNodeType)aReader.ReadByte();
                switch (type)
                {
                    case JSONNodeType.Array:
                        {
                            int count = aReader.ReadInt32();
                            JSONArray tmp = new JSONArray();
                            for (int i = 0; i < count; i++)
                                tmp.Add(Deserialize(aReader));
                            return tmp;
                        }
                    case JSONNodeType.Object:
                        {
                            int count = aReader.ReadInt32();
                            JSONObject tmp = new JSONObject();
                            for (int i = 0; i < count; i++)
                            {
                                string key = aReader.ReadString();
                                var val = Deserialize(aReader);
                                tmp.Add(key, val);
                            }
                            return tmp;
                        }
                    case JSONNodeType.String:
                        {
                            return new JSONString(aReader.ReadString());
                        }
                    case JSONNodeType.Number:
                        {
                            return new JSONNumber(aReader.ReadDouble());
                        }
                    case JSONNodeType.Boolean:
                        {
                            return new JSONBool(aReader.ReadBoolean());
                        }
                    case JSONNodeType.NullValue:
                        {
                            return new JSONNull();
                        }
                    default:
                        {
                            throw new Exception("Error deserializing JSON. Unknown tag: " + type);
                        }
                }
            }

#if USE_SharpZipLib
		public static JSONNode LoadFromCompressedStream(System.IO.Stream aData)
		{
			var zin = new ICSharpCode.SharpZipLib.BZip2.BZip2InputStream(aData);
			return LoadFromStream(zin);
		}
		public static JSONNode LoadFromCompressedFile(string aFileName)
		{
#if USE_FileIO
			using(var F = System.IO.File.OpenRead(aFileName))
			{
				return LoadFromCompressedStream(F);
			}
#else
			throw new Exception("Can't use File IO stuff in the webplayer");
#endif
		}
		public static JSONNode LoadFromCompressedBase64(string aBase64)
		{
			var tmp = System.Convert.FromBase64String(aBase64);
			var stream = new System.IO.MemoryStream(tmp);
			stream.Position = 0;
			return LoadFromCompressedStream(stream);
		}
#else
            public static JSONNode LoadFromCompressedFile(string aFileName)
            {
                throw new Exception("Can't use compressed functions. You need include the SharpZipLib and uncomment the define at the top of SimpleJSON");
            }

            public static JSONNode LoadFromCompressedStream(System.IO.Stream aData)
            {
                throw new Exception("Can't use compressed functions. You need include the SharpZipLib and uncomment the define at the top of SimpleJSON");
            }

            public static JSONNode LoadFromCompressedBase64(string aBase64)
            {
                throw new Exception("Can't use compressed functions. You need include the SharpZipLib and uncomment the define at the top of SimpleJSON");
            }
#endif

            public static JSONNode LoadFromStream(System.IO.Stream aData)
            {
                using (var R = new System.IO.BinaryReader(aData))
                {
                    return Deserialize(R);
                }
            }

            public static JSONNode LoadFromFile(string aFileName)
            {
#if USE_FileIO
            using (var F = System.IO.File.OpenRead(aFileName))
            {
                return LoadFromStream(F);
            }
#else
                throw new Exception("Can't use File IO stuff in the webplayer");
#endif
            }

            public static JSONNode LoadFromBase64(string aBase64)
            {
                var tmp = System.Convert.FromBase64String(aBase64);
                var stream = new System.IO.MemoryStream(tmp);
                stream.Position = 0;
                return LoadFromStream(stream);
            }
        }
        // End of JSONNode

        public class JSONArray : JSONNode, IEnumerable
        {
            private List<JSONNode> m_List = new List<JSONNode>();
            public bool inline = false;

            public override JSONNodeType Tag { get { return JSONNodeType.Array; } }
            public override bool IsArray { get { return true; } }

            public override JSONNode this[int aIndex]
            {
                get
                {
                    if (aIndex < 0 || aIndex >= m_List.Count)
                        return new JSONLazyCreator(this);
                    return m_List[aIndex];
                }
                set
                {
                    if (value == null)
                        value = new JSONNull();
                    if (aIndex < 0 || aIndex >= m_List.Count)
                        m_List.Add(value);
                    else
                        m_List[aIndex] = value;
                }
            }

            public override JSONNode this[string aKey]
            {
                get { return new JSONLazyCreator(this); }
                set
                {
                    if (value == null)
                        value = new JSONNull();
                    m_List.Add(value);
                }
            }

            public override int Count
            {
                get { return m_List.Count; }
            }

            public override void Add(string aKey, JSONNode aItem)
            {
                if (aItem == null)
                    aItem = new JSONNull();
                m_List.Add(aItem);
            }

            public override JSONNode Remove(int aIndex)
            {
                if (aIndex < 0 || aIndex >= m_List.Count)
                    return null;
                JSONNode tmp = m_List[aIndex];
                m_List.RemoveAt(aIndex);
                return tmp;
            }

            public override JSONNode Remove(JSONNode aNode)
            {
                m_List.Remove(aNode);
                return aNode;
            }

            public override IEnumerable<JSONNode> Children
            {
                get
                {
                    foreach (JSONNode N in m_List)
                        yield return N;
                }
            }

            public IEnumerator GetEnumerator()
            {
                foreach (JSONNode N in m_List)
                    yield return N;
            }

            public override void Serialize(System.IO.BinaryWriter aWriter)
            {
                aWriter.Write((byte)JSONNodeType.Array);
                aWriter.Write(m_List.Count);
                for (int i = 0; i < m_List.Count; i++)
                {
                    m_List[i].Serialize(aWriter);
                }
            }

            internal override void WriteToStringBuilder(StringBuilder aSB, int aIndent, int aIndentInc, JSONTextMode aMode)
            {
                aSB.Append('[');
                int count = m_List.Count;
                if (inline)
                    aMode = JSONTextMode.Compact;
                for (int i = 0; i < count; i++)
                {
                    if (i > 0)
                        aSB.Append(',');
                    if (aMode == JSONTextMode.Indent)
                        aSB.AppendLine();

                    if (aMode == JSONTextMode.Indent)
                        aSB.Append(' ', aIndent + aIndentInc);
                    m_List[i].WriteToStringBuilder(aSB, aIndent + aIndentInc, aIndentInc, aMode);
                }
                if (aMode == JSONTextMode.Indent)
                    aSB.AppendLine().Append(' ', aIndent);
                aSB.Append(']');
            }
        }
        // End of JSONArray

        public class JSONObject : JSONNode, IEnumerable
        {
            private Dictionary<string, JSONNode> m_Dict = new Dictionary<string, JSONNode>();

            public bool inline = false;

            public override JSONNodeType Tag { get { return JSONNodeType.Object; } }
            public override bool IsObject { get { return true; } }


            public override JSONNode this[string aKey]
            {
                get
                {
                    if (m_Dict.ContainsKey(aKey))
                        return m_Dict[aKey];
                    else
                        return new JSONLazyCreator(this, aKey);
                }
                set
                {
                    if (value == null)
                        value = new JSONNull();
                    if (m_Dict.ContainsKey(aKey))
                        m_Dict[aKey] = value;
                    else
                        m_Dict.Add(aKey, value);
                }
            }

            public override JSONNode this[int aIndex]
            {
                get
                {
                    if (aIndex < 0 || aIndex >= m_Dict.Count)
                        return null;
                    return m_Dict.ElementAt(aIndex).Value;
                }
                set
                {
                    if (value == null)
                        value = new JSONNull();
                    if (aIndex < 0 || aIndex >= m_Dict.Count)
                        return;
                    string key = m_Dict.ElementAt(aIndex).Key;
                    m_Dict[key] = value;
                }
            }

            public override int Count
            {
                get { return m_Dict.Count; }
            }

            public override void Add(string aKey, JSONNode aItem)
            {
                if (aItem == null)
                    aItem = new JSONNull();

                if (!string.IsNullOrEmpty(aKey))
                {
                    if (m_Dict.ContainsKey(aKey))
                        m_Dict[aKey] = aItem;
                    else
                        m_Dict.Add(aKey, aItem);
                }
                else
                    m_Dict.Add(Guid.NewGuid().ToString(), aItem);
            }

            public override JSONNode Remove(string aKey)
            {
                if (!m_Dict.ContainsKey(aKey))
                    return null;
                JSONNode tmp = m_Dict[aKey];
                m_Dict.Remove(aKey);
                return tmp;
            }

            public override JSONNode Remove(int aIndex)
            {
                if (aIndex < 0 || aIndex >= m_Dict.Count)
                    return null;
                var item = m_Dict.ElementAt(aIndex);
                m_Dict.Remove(item.Key);
                return item.Value;
            }

            public override JSONNode Remove(JSONNode aNode)
            {
                try
                {
                    var item = m_Dict.Where(k => k.Value == aNode).First();
                    m_Dict.Remove(item.Key);
                    return aNode;
                }
                catch
                {
                    return null;
                }
            }

            public override IEnumerable<JSONNode> Children
            {
                get
                {
                    foreach (KeyValuePair<string, JSONNode> N in m_Dict)
                        yield return N.Value;
                }
            }

            public IEnumerator GetEnumerator()
            {
                foreach (KeyValuePair<string, JSONNode> N in m_Dict)
                    yield return N;
            }

            public override void Serialize(System.IO.BinaryWriter aWriter)
            {
                aWriter.Write((byte)JSONNodeType.Object);
                aWriter.Write(m_Dict.Count);
                foreach (string K in m_Dict.Keys)
                {
                    aWriter.Write(K);
                    m_Dict[K].Serialize(aWriter);
                }
            }
            internal override void WriteToStringBuilder(StringBuilder aSB, int aIndent, int aIndentInc, JSONTextMode aMode)
            {
                aSB.Append('{');
                bool first = true;
                if (inline)
                    aMode = JSONTextMode.Compact;
                foreach (var k in m_Dict)
                {
                    if (!first)
                        aSB.Append(',');
                    first = false;
                    if (aMode == JSONTextMode.Indent)
                        aSB.AppendLine();
                    if (aMode == JSONTextMode.Indent)
                        aSB.Append(' ', aIndent + aIndentInc);
                    aSB.Append('\"').Append(Escape(k.Key)).Append('\"');
                    if (aMode == JSONTextMode.Compact)
                        aSB.Append(':');
                    else
                        aSB.Append(" : ");
                    k.Value.WriteToStringBuilder(aSB, aIndent + aIndentInc, aIndentInc, aMode);
                }
                if (aMode == JSONTextMode.Indent)
                    aSB.AppendLine().Append(' ', aIndent);
                aSB.Append('}');
            }

        }
        // End of JSONObject

        public class JSONString : JSONNode
        {
            private string m_Data;

            public override JSONNodeType Tag { get { return JSONNodeType.String; } }
            public override bool IsString { get { return true; } }

            public override string Value
            {
                get { return m_Data; }
                set
                {
                    m_Data = value;
                }
            }

            public JSONString(string aData)
            {
                m_Data = aData;
            }

            public override void Serialize(System.IO.BinaryWriter aWriter)
            {
                aWriter.Write((byte)JSONNodeType.String);
                aWriter.Write(m_Data);
            }
            internal override void WriteToStringBuilder(StringBuilder aSB, int aIndent, int aIndentInc, JSONTextMode aMode)
            {
                aSB.Append('\"').Append(Escape(m_Data)).Append('\"');
            }
            public override bool Equals(object obj)
            {
                if (base.Equals(obj))
                    return true;
                string s = obj as string;
                if (s != null)
                    return m_Data == s;
                JSONString s2 = obj as JSONString;
                if (s2 != null)
                    return m_Data == s2.m_Data;
                return false;
            }
            public override int GetHashCode()
            {
                return m_Data.GetHashCode();
            }
        }
        // End of JSONString

        public class JSONNumber : JSONNode
        {
            private double m_Data;

            public override JSONNodeType Tag { get { return JSONNodeType.Number; } }
            public override bool IsNumber { get { return true; } }


            public override string Value
            {
                get { return m_Data.ToString(); }
                set
                {
                    double v;
                    if (double.TryParse(value, out v))
                        m_Data = v;
                }
            }

            public override double AsDouble
            {
                get { return m_Data; }
                set { m_Data = value; }
            }

            public JSONNumber(double aData)
            {
                m_Data = aData;
            }

            public JSONNumber(string aData)
            {
                Value = aData;
            }

            public override void Serialize(System.IO.BinaryWriter aWriter)
            {
                aWriter.Write((byte)JSONNodeType.Number);
                aWriter.Write(m_Data);
            }
            internal override void WriteToStringBuilder(StringBuilder aSB, int aIndent, int aIndentInc, JSONTextMode aMode)
            {
                aSB.Append(m_Data);
            }
            private static bool IsNumeric(object value)
            {
                return value is int || value is uint
                    || value is float || value is double
                    || value is decimal
                    || value is long || value is ulong
                    || value is short || value is ushort
                    || value is sbyte || value is byte;
            }
            public override bool Equals(object obj)
            {
                if (obj == null)
                    return false;
                if (base.Equals(obj))
                    return true;
                JSONNumber s2 = obj as JSONNumber;
                if (s2 != null)
                    return m_Data == s2.m_Data;
                if (IsNumeric(obj))
                    return Convert.ToDouble(obj) == m_Data;
                return false;
            }
            public override int GetHashCode()
            {
                return m_Data.GetHashCode();
            }
        }
        // End of JSONNumber

        public class JSONBool : JSONNode
        {
            private bool m_Data;

            public override JSONNodeType Tag { get { return JSONNodeType.Boolean; } }
            public override bool IsBoolean { get { return true; } }


            public override string Value
            {
                get { return m_Data.ToString(); }
                set
                {
                    bool v;
                    if (bool.TryParse(value, out v))
                        m_Data = v;
                }
            }
            public override bool AsBool
            {
                get { return m_Data; }
                set { m_Data = value; }
            }

            public JSONBool(bool aData)
            {
                m_Data = aData;
            }

            public JSONBool(string aData)
            {
                Value = aData;
            }

            public override void Serialize(System.IO.BinaryWriter aWriter)
            {
                aWriter.Write((byte)JSONNodeType.Boolean);
                aWriter.Write(m_Data);
            }
            internal override void WriteToStringBuilder(StringBuilder aSB, int aIndent, int aIndentInc, JSONTextMode aMode)
            {
                aSB.Append((m_Data) ? "true" : "false");
            }
            public override bool Equals(object obj)
            {
                if (obj == null)
                    return false;
                if (obj is bool)
                    return m_Data == (bool)obj;
                return false;
            }
            public override int GetHashCode()
            {
                return m_Data.GetHashCode();
            }
        }
        // End of JSONBool

        public class JSONNull : JSONNode
        {

            public override JSONNodeType Tag { get { return JSONNodeType.NullValue; } }
            public override bool IsNull { get { return true; } }

            public override string Value
            {
                get { return "null"; }
                set { }
            }
            public override bool AsBool
            {
                get { return false; }
                set { }
            }

            public override bool Equals(object obj)
            {
                if (object.ReferenceEquals(this, obj))
                    return true;
                return (obj is JSONNull);
            }
            public override int GetHashCode()
            {
                return 0;
            }

            public override void Serialize(System.IO.BinaryWriter aWriter)
            {
                aWriter.Write((byte)JSONNodeType.NullValue);
            }
            internal override void WriteToStringBuilder(StringBuilder aSB, int aIndent, int aIndentInc, JSONTextMode aMode)
            {
                aSB.Append("null");
            }
        }
        // End of JSONNull

        internal class JSONLazyCreator : JSONNode
        {
            private JSONNode m_Node = null;
            private string m_Key = null;

            public override JSONNodeType Tag { get { return JSONNodeType.None; } }

            public JSONLazyCreator(JSONNode aNode)
            {
                m_Node = aNode;
                m_Key = null;
            }

            public JSONLazyCreator(JSONNode aNode, string aKey)
            {
                m_Node = aNode;
                m_Key = aKey;
            }

            private void Set(JSONNode aVal)
            {
                if (m_Key == null)
                {
                    m_Node.Add(aVal);
                }
                else
                {
                    m_Node.Add(m_Key, aVal);
                }
                m_Node = null; // Be GC friendly.
            }

            public override JSONNode this[int aIndex]
            {
                get
                {
                    return new JSONLazyCreator(this);
                }
                set
                {
                    var tmp = new JSONArray();
                    tmp.Add(value);
                    Set(tmp);
                }
            }

            public override JSONNode this[string aKey]
            {
                get
                {
                    return new JSONLazyCreator(this, aKey);
                }
                set
                {
                    var tmp = new JSONObject();
                    tmp.Add(aKey, value);
                    Set(tmp);
                }
            }

            public override void Add(JSONNode aItem)
            {
                var tmp = new JSONArray();
                tmp.Add(aItem);
                Set(tmp);
            }

            public override void Add(string aKey, JSONNode aItem)
            {
                var tmp = new JSONObject();
                tmp.Add(aKey, aItem);
                Set(tmp);
            }

            public static bool operator ==(JSONLazyCreator a, object b)
            {
                if (b == null)
                    return true;
                return System.Object.ReferenceEquals(a, b);
            }

            public static bool operator !=(JSONLazyCreator a, object b)
            {
                return !(a == b);
            }

            public override bool Equals(object obj)
            {
                if (obj == null)
                    return true;
                return System.Object.ReferenceEquals(this, obj);
            }

            public override int GetHashCode()
            {
                return 0;
            }

            public override int AsInt
            {
                get
                {
                    JSONNumber tmp = new JSONNumber(0);
                    Set(tmp);
                    return 0;
                }
                set
                {
                    JSONNumber tmp = new JSONNumber(value);
                    Set(tmp);
                }
            }

            public override float AsFloat
            {
                get
                {
                    JSONNumber tmp = new JSONNumber(0.0f);
                    Set(tmp);
                    return 0.0f;
                }
                set
                {
                    JSONNumber tmp = new JSONNumber(value);
                    Set(tmp);
                }
            }

            public override double AsDouble
            {
                get
                {
                    JSONNumber tmp = new JSONNumber(0.0);
                    Set(tmp);
                    return 0.0;
                }
                set
                {
                    JSONNumber tmp = new JSONNumber(value);
                    Set(tmp);
                }
            }

            public override bool AsBool
            {
                get
                {
                    JSONBool tmp = new JSONBool(false);
                    Set(tmp);
                    return false;
                }
                set
                {
                    JSONBool tmp = new JSONBool(value);
                    Set(tmp);
                }
            }

            public override JSONArray AsArray
            {
                get
                {
                    JSONArray tmp = new JSONArray();
                    Set(tmp);
                    return tmp;
                }
            }

            public override JSONObject AsObject
            {
                get
                {
                    JSONObject tmp = new JSONObject();
                    Set(tmp);
                    return tmp;
                }
            }
            internal override void WriteToStringBuilder(StringBuilder aSB, int aIndent, int aIndentInc, JSONTextMode aMode)
            {
                aSB.Append("null");
            }
        }
        // End of JSONLazyCreator

        public static class JSON
        {
            public static JSONNode Parse(string aJSON)
            {
                return JSONNode.Parse(aJSON);
            }
        }
    }

    namespace Request
    {

        #region DefaultClass
        public class User
        {
            public int id;
            public string first_name;
            public string last_name;
            public string username;
        }
        public class Chat
        {
            public int id;
            public string type;
            public string title;
            public string first_name;
            public string last_name;
            public string username;
        }
        public class PhotoSize
        {
            public string file_id;
            public int width;
            public int height;
            public int file_size;
        }
        #endregion
        #region Delegates
        public delegate void ResponseText(object sendr, MessageText e);
        public delegate void ResponseSticker(object sendr, MessageSticker e);
        public delegate void ResponsePhoto(object sendr, MessagePhoto e);
        public delegate void ResponseVideo(object sendr, MessageVideo e);
        public delegate void ResponseDocument(object sendr, MessageDocument e);
        public delegate void ResponseLocation(object sendr, MessageLocation e);
        public delegate void ResponseContact(object sendr, MessageContact e);
        public delegate void ResponseVoice(object sendr, MessageVoice e);
        #endregion
        #region Classes
        public class MessageText : EventArgs
        {
            //public string name;
            //public string message;
            //public string chatID;
            #region DefaultParameters
            public int message_id;
            public User from = new User();
            public Chat chat = new Chat();
            public int date;
            #endregion
            public string text;
        }
        public class MessageSticker : EventArgs
        {
            #region DefaulParameters
            public int message_id;
            public User from = new User();
            public Chat chat = new Chat();
            public int date;
            #endregion
            public string file_id;
            public int width;
            public int height;
            public PhotoSize thumb = new PhotoSize();
            public string emoji;
            public int file_size;
        }
        public class MessagePhoto : EventArgs
        {
            #region DefaulParameters
            public int message_id;
            public User from = new User();
            public Chat chat = new Chat();
            public int date;
            #endregion
            public List<PhotoSize> photo = new List<PhotoSize>();
            public string caption;
        }
        public class MessageVideo : EventArgs
        {
            #region DefaulParameters
            public int message_id;
            public User from = new User();
            public Chat chat = new Chat();
            public int date;
            #endregion
            public string file_id;
            public int width;
            public int height;
            public int duration;
            public PhotoSize thumb = new PhotoSize();
            public string mime_type;
            public int file_size;
        }
        public class MessageDocument : EventArgs
        {
            #region DefaulParameters
            public int message_id;
            public User from = new User();
            public Chat chat = new Chat();
            public int date;
            #endregion
            public string file_id;
            public PhotoSize thumb = new PhotoSize();
            public string file_name;
            public string mime_type;
            public int file_size;
        }
        public class MessageLocation : EventArgs
        {
            #region DefaulParameters
            public int message_id;
            public User from = new User();
            public Chat chat = new Chat();
            public int date;
            #endregion
            public float longitude;
            public float latitude;
        }
        public class MessageContact : EventArgs
        {
            #region DefaulParameters
            public int message_id;
            public User from = new User();
            public Chat chat = new Chat();
            public int date;
            #endregion
            public string phone_number;
            public string first_name;
            public string last_name;
            public int user_id;
        }
        public class MessageVoice : EventArgs
        {
            #region DefaulParameters
            public int message_id;
            public User from = new User();
            public Chat chat = new Chat();
            public int date;
            #endregion
            public string file_id;
            public int duration;
            public string mime_type;
            public int file_size;
        }
        #endregion 

    public class TelegramRequest
        {
            public string _token;
            public TelegramRequest(string Token)
            {
                _token = Token;
            }
            int LastUpdateID = 0;

            public event ResponseText MessageText;
            public event ResponseSticker MessageSticker;
            public event ResponsePhoto MessagePhoto;
            public event ResponseVideo MessageVideo;
            public event ResponseDocument MessageDocument;
            public event ResponseLocation MessageLocation;
            public event ResponseContact MessageContact;
            public event ResponseVoice MessageVoice;
            
            MessageText e = new MessageText();

            public void GetUpdates()
            {
                while (true)
                {
                    using (WebClient webClient = new WebClient())
                    {
                        string response = webClient.DownloadString("https://api.telegram.org/bot" + _token + "/getupdates?offset=" + (LastUpdateID + 1));
                        if (response.Length <= 23)
                            continue;
                        var N = JSON.Parse(response);
                        foreach (JSONNode r in N["result"].AsArray)
                        {
                            string _type = r["message"].ToString();
                            _type = WhatsType(_type).Replace("\"", "");
                            LastUpdateID = r["update_id"].AsInt;
                            #region SWITCH
                            switch (_type)
                            {
                                
                                case "text":
                                    {
                                        try
                                        {
                                            MessageText.Method.Name.ToString();
                                            //Notification();
                                            //Console.WriteLine("Recieved a text message");
                                            //Notification(false);                                            
                                            
                                        }
                                        catch (Exception)
                                        { break; }
                                        GetMessageText(r);
                                        break;
                                    }
                                case "sticker":
                                    {
                                        try
                                        {
                                            MessageText.Method.Name.ToString();
                                            Notification();
                                            Console.WriteLine("Recieved a sticker");
                                            Notification(false);
                                        }
                                        catch (Exception)
                                        { break; }
                                        GetMessageSticker(r);
                                        break;
                                    }
                                case "photo":
                                    {
                                        try
                                        {
                                            MessageText.Method.Name.ToString();
                                            Notification();
                                            Console.WriteLine("Recieved a photo");
                                            Notification(false);
                                        }
                                        catch (Exception)
                                        { break; }
                                        GetMessagePhoto(r);
                                        break;
                                    }
                                case "video":
                                    {
                                        try
                                        {
                                            MessageText.Method.Name.ToString();
                                            Notification();
                                            Console.WriteLine("Recieved a video");
                                            Notification(false);
                                        }
                                        catch (Exception)
                                        { break; }
                                        GetMessageVideo(r);
                                        break;
                                    }
                                case "document":
                                    {
                                        try
                                        {
                                            MessageText.Method.Name.ToString();
                                            Notification();
                                            Console.WriteLine("Recieved a document");
                                            Notification(false);
                                        }
                                        catch (Exception)
                                        { break; }
                                        GetMessageDocument(r);
                                        break;
                                    }
                                case "location":
                                    {
                                        try
                                        {
                                            MessageText.Method.Name.ToString();
                                            Notification();
                                            Console.WriteLine("Recieved a location");
                                            Notification(false);
                                        }
                                        catch (Exception)
                                        { break; }
                                        GetMessageLocation(r);
                                        break;
                                    }
                                case "contact":
                                    {
                                        try
                                        {
                                            MessageText.Method.Name.ToString();
                                            Notification();
                                            Console.WriteLine("Recieved a contact");
                                            Notification(false);
                                        }
                                        catch (Exception)
                                        { break; }
                                        GetMessageContact(r);
                                        break;
                                    }
                                case "voice":
                                    {
                                        try
                                        {
                                            MessageText.Method.Name.ToString();
                                            Notification();
                                            Console.WriteLine("Recieved a voice");
                                            Notification(false);
                                        }
                                        catch (Exception)
                                        { break; }
                                        GetMessageVoice(r);
                                        break;
                                    }
                            }
                            #endregion
                        }
                    }
                }
            }

            private string WhatsType(string JSON)
            {
                string[] Type = { "text", "sticker", "photo", "video", "document", "location", "contact", "voice" };
                for (int i = 0; i < Type.Length; i++)
                {
                    try { JSON = JSON.Remove(0, JSON.LastIndexOf("\"" + Type[i] + "\"")); }
                    catch (Exception) { continue; }
                    JSON = JSON.Remove(JSON.IndexOf(":"));
                    break;
                }
                return JSON;
            }

            private void Notification(bool on = true)
            {
                if (on)
                {
                    Console.BackgroundColor = ConsoleColor.Black;
                    Console.BackgroundColor = ConsoleColor.DarkRed;
                }
                else
                {
                    Console.BackgroundColor = ConsoleColor.White;
                    Console.BackgroundColor = ConsoleColor.Black;
                }
            }

            #region Method
            private void GetMessageText(JSONNode r)
            {
                MessageText message = new Request.MessageText();
                #region GetDefaultInformation
                //id
                message.message_id = r["message"]["message_id"].AsInt;
                //from
                message.from.id = r["message"]["from"]["id"].AsInt;
                message.from.first_name = r["message"]["from"]["first_name"];
                message.from.last_name = r["message"]["from"]["last_name"];
                message.from.username = r["message"]["from"]["username"];
                //chat
                message.chat.id = r["message"]["chat"]["id"].AsInt;
                message.chat.first_name = r["message"]["chat"]["first_name"];
                message.chat.last_name = r["message"]["chat"]["last_name"];
                message.chat.username = r["message"]["chat"]["username"];
                message.chat.type = r["message"]["chat"]["type"];
                message.chat.title = r["message"]["chat"]["title"];
                //date
                message.date = r["message"]["date"];
                //text
                message.text = r["message"]["text"];
                //event
                MessageText(this, message);
                #endregion

            }
            private void GetMessageSticker(JSONNode r)
            {
                MessageSticker message = new MessageSticker();
                #region GetDefaultInformation
                //message_id
                message.message_id = r["message"]["message_id"].AsInt;
                //from
                message.from.id = r["message"]["from"]["id"].AsInt;
                message.from.first_name = r["message"]["from"]["first_name"];
                message.from.last_name = r["message"]["from"]["last_name"];
                message.from.username = r["message"]["from"]["username"];
                //chat
                message.chat.id = r["message"]["chat"]["id"].AsInt;
                message.chat.first_name = r["message"]["chat"]["first_name"];
                message.chat.last_name = r["message"]["chat"]["last_name"];
                message.chat.username = r["message"]["chat"]["username"];
                message.chat.type = r["message"]["chat"]["type"];
                message.chat.title = r["message"]["chat"]["title"];
                //date
                message.date = r["message"]["date"].AsInt;
                #endregion
                //sticker
                message.width = r["message"]["sticker"]["width"].AsInt;
                message.height = r["message"]["sticker"]["height"].AsInt;
                message.emoji = r["message"]["sticker"]["height"];
                //thumb
                message.thumb.file_id = r["message"]["sticker"]["thumb"]["file_id"];
                message.thumb.file_size = r["message"]["sticker"]["thumb"]["file_size"].AsInt;
                message.thumb.width = r["message"]["sticker"]["thumb"]["width"].AsInt;
                message.thumb.height = r["message"]["sticker"]["thumb"]["height"].AsInt;
                //ene thumb
                message.file_id = r["message"]["sticker"]["file_id"];
                message.file_size = r["message"]["sticker"]["file_size"].AsInt;
                //Event
                MessageSticker(this, message);
            }
            private void GetMessagePhoto(JSONNode r)
            {
                MessagePhoto message = new MessagePhoto();
                #region GetDefaultInformation
                //message_id
                message.message_id = r["message"]["message_id"].AsInt;
                //from
                message.from.id = r["message"]["from"]["id"].AsInt;
                message.from.first_name = r["message"]["from"]["first_name"];
                message.from.last_name = r["message"]["from"]["last_name"];
                message.from.username = r["message"]["from"]["username"];
                //chat
                message.chat.id = r["message"]["chat"]["id"].AsInt;
                message.chat.first_name = r["message"]["chat"]["first_name"];
                message.chat.last_name = r["message"]["chat"]["last_name"];
                message.chat.username = r["message"]["chat"]["username"];
                message.chat.type = r["message"]["chat"]["type"];
                message.chat.title = r["message"]["chat"]["title"];
                //date
                message.date = r["message"]["date"].AsInt;
                #endregion
                //sticker
                for (int i = 0; i < r["message"]["photo"].Count; i++)
                {
                    message.photo.Add(new PhotoSize
                    {
                        file_id = r["message"]["photo"][i]["file_id"],
                        file_size = r["message"]["photo"][i]["file_size"].AsInt,
                        height = r["message"]["photo"][i]["height"].AsInt,
                        width = r["message"]["photo"][i]["width"].AsInt
                    });
                }
                message.caption = r["message"]["caption"];
                //Event
                MessagePhoto(this, message);
            }
            private void GetMessageVideo(JSONNode r)
            {
                MessageVideo message = new MessageVideo();
                #region GetDefaultInformation
                //message_id
                message.message_id = r["message"]["message_id"].AsInt;
                //from
                message.from.id = r["message"]["from"]["id"].AsInt;
                message.from.first_name = r["message"]["from"]["first_name"];
                message.from.last_name = r["message"]["from"]["last_name"];
                message.from.username = r["message"]["from"]["username"];
                //chat
                message.chat.id = r["message"]["chat"]["id"].AsInt;
                message.chat.first_name = r["message"]["chat"]["first_name"];
                message.chat.last_name = r["message"]["chat"]["last_name"];
                message.chat.username = r["message"]["chat"]["username"];
                message.chat.type = r["message"]["chat"]["type"];
                message.chat.title = r["message"]["chat"]["title"];
                //date
                message.date = r["message"]["date"].AsInt;
                #endregion
                //video
                message.file_id = r["message"]["video"]["file_id"];
                message.duration = r["message"]["video"]["duration"].AsInt;
                message.width = r["message"]["video"]["width"].AsInt;
                message.height = r["message"]["video"]["height"].AsInt;
                //video-thum
                message.thumb.file_id = r["message"]["video"]["thumb"]["file_id"];
                message.thumb.file_size = r["message"]["video"]["thumb"]["file_size"].AsInt;
                message.thumb.width = r["message"]["video"]["thumb"]["width"].AsInt;
                message.thumb.height = r["message"]["video"]["thumb"]["height"].AsInt;
                //thumb end
                message.mime_type = r["message"]["video"]["mime_type"];
                message.file_size = r["message"]["video"]["file_size"].AsInt;
                //Event
                MessageVideo(this, message);
            }
            private void GetMessageDocument(JSONNode r)
            {
                MessageDocument message = new MessageDocument();
                #region GetDefaultInformation
                //message_id
                message.message_id = r["message"]["message_id"].AsInt;
                //from
                message.from.id = r["message"]["from"]["id"].AsInt;
                message.from.first_name = r["message"]["from"]["first_name"];
                message.from.last_name = r["message"]["from"]["last_name"];
                message.from.username = r["message"]["from"]["username"];
                //chat
                message.chat.id = r["message"]["chat"]["id"].AsInt;
                message.chat.first_name = r["message"]["chat"]["first_name"];
                message.chat.last_name = r["message"]["chat"]["last_name"];
                message.chat.username = r["message"]["chat"]["username"];
                message.chat.type = r["message"]["chat"]["type"];
                message.chat.title = r["message"]["chat"]["title"];
                //date
                message.date = r["message"]["date"].AsInt;
                #endregion
                //document
                message.file_id = r["message"]["document"]["file_id"];
                message.file_name = r["message"]["document"]["file_name"];
                message.mime_type = r["message"]["document"]["mime_type"];
                message.file_size = r["message"]["document"]["file_size"].AsInt;
                //document-thumb
                message.thumb.file_id = r["message"]["document"]["thumb"]["file_id"];
                message.thumb.file_size = r["message"]["document"]["thumb"]["file_size"].AsInt;
                message.thumb.width = r["message"]["document"]["thumb"]["width"].AsInt;
                message.thumb.height = r["message"]["document"]["thumb"]["height"].AsInt;
                //thumb end
                //Event
                MessageDocument(this, message);
            }
            private void GetMessageLocation(JSONNode r)
            {
                MessageLocation message = new MessageLocation();
                #region GetDefaultInformation
                //message_id
                message.message_id = r["message"]["message_id"].AsInt;
                //from
                message.from.id = r["message"]["from"]["id"].AsInt;
                message.from.first_name = r["message"]["from"]["first_name"];
                message.from.last_name = r["message"]["from"]["last_name"];
                message.from.username = r["message"]["from"]["username"];
                //chat
                message.chat.id = r["message"]["chat"]["id"].AsInt;
                message.chat.first_name = r["message"]["chat"]["first_name"];
                message.chat.last_name = r["message"]["chat"]["last_name"];
                message.chat.username = r["message"]["chat"]["username"];
                message.chat.type = r["message"]["chat"]["type"];
                message.chat.title = r["message"]["chat"]["title"];
                //date
                message.date = r["message"]["date"].AsInt;
                #endregion
                //location
                message.longitude = r["message"]["location"]["longitude"].AsFloat;
                message.latitude = r["message"]["location"]["latitude"].AsFloat;
                //Event
                MessageLocation(this, message);
            }
            private void GetMessageContact(JSONNode r)
            {
                MessageContact message = new MessageContact();
                #region GetDefaultInformation
                //message_id
                message.message_id = r["message"]["message_id"].AsInt;
                //from
                message.from.id = r["message"]["from"]["id"].AsInt;
                message.from.first_name = r["message"]["from"]["first_name"];
                message.from.last_name = r["message"]["from"]["last_name"];
                message.from.username = r["message"]["from"]["username"];
                //chat
                message.chat.id = r["message"]["chat"]["id"].AsInt;
                message.chat.first_name = r["message"]["chat"]["first_name"];
                message.chat.last_name = r["message"]["chat"]["last_name"];
                message.chat.username = r["message"]["chat"]["username"];
                message.chat.type = r["message"]["chat"]["type"];
                message.chat.title = r["message"]["chat"]["title"];
                //date
                message.date = r["message"]["date"].AsInt;
                #endregion
                //contact
                message.phone_number = r["message"]["contact"]["phone_number"];
                message.first_name = r["message"]["contact"]["first_name"];
                message.last_name = r["message"]["contact"]["last_name"];
                message.user_id = r["message"]["contact"]["user_id"].AsInt;
                //Event
                MessageContact(this, message);
            }
            private void GetMessageVoice(JSONNode r)
            {
                MessageVoice message = new MessageVoice();
                #region GetDefaultInformation
                //message_id
                message.message_id = r["message"]["message_id"].AsInt;
                //from
                message.from.id = r["message"]["from"]["id"].AsInt;
                message.from.first_name = r["message"]["from"]["first_name"];
                message.from.last_name = r["message"]["from"]["last_name"];
                message.from.username = r["message"]["from"]["username"];
                //chat
                message.chat.id = r["message"]["chat"]["id"].AsInt;
                message.chat.first_name = r["message"]["chat"]["first_name"];
                message.chat.last_name = r["message"]["chat"]["last_name"];
                message.chat.username = r["message"]["chat"]["username"];
                message.chat.type = r["message"]["chat"]["type"];
                message.chat.title = r["message"]["chat"]["title"];
                //date
                message.date = r["message"]["date"].AsInt;
                #endregion
                //voice
                message.file_id = r["message"]["voice"]["file_id"];
                message.duration = r["message"]["voice"]["duration"].AsInt;
                message.mime_type = r["message"]["voice"]["mime_type"];
                message.file_size = r["message"]["voice"]["file_size"].AsInt;
                //Event
                MessageVoice(this, message);
            }            
            #endregion

        }
    }

    class Method
    {
        string _token;
        string LINK = "https://api.telegram.org/bot";

        public Method(string Token)
        {
            _token = Token;
        }

        //------
        //------
        //------Functions

        public string Getme()
        {
            using (WebClient webClient = new WebClient())
            {
                string response = webClient.DownloadString(LINK + _token + "/getMe");
                return response;
            }
        }

        public void SendMessage(string message, int ChatID)
        {
            using (WebClient webClient = new WebClient())
            {
                NameValueCollection pars = new NameValueCollection();
                pars.Add("chat_id", ChatID.ToString());
                pars.Add("text", message);
                webClient.UploadValues(LINK + _token + "/sendMessage", pars);
            }
        }

        public void ForwardMessage(int fromChatID, int chatId, int messageID)
        {
            using (WebClient webClient = new WebClient())
            {
                NameValueCollection pars = new NameValueCollection();
                pars.Add("chat_id", chatId.ToString());
                pars.Add("from_chat_id", fromChatID.ToString());
                pars.Add("message_id", messageID.ToString());
                webClient.UploadValues(LINK + _token + "/forwardMessage", pars);
            }
        }

        async public Task SendPhotoIputFile(int ChatID, string pathToPhoto, string caption = "")
        {
            using (MultipartFormDataContent form = new MultipartFormDataContent())
            {
                string url = LINK + _token + "/sendPhoto";
                string fileName = pathToPhoto.Split('\\').Last();

                form.Add(new StringContent(ChatID.ToString(), Encoding.UTF8), "chat_id");
                form.Add(new StringContent(caption.ToString(), Encoding.UTF8), "caption");
                using (FileStream fileStream = new FileStream(pathToPhoto, FileMode.Open, FileAccess.Read))
                {
                    form.Add(new StreamContent(fileStream), "photo", fileName);
                    using (HttpClient client = new HttpClient())
                        await client.PostAsync(url, form);
                }
            }

        }
        public void SendPhotoLink(int ChatID, string linkToPhoto, string caption = "")
        {
            using (WebClient webClient = new WebClient())
            {
                NameValueCollection pars = new NameValueCollection();
                pars.Add("chat_id", ChatID.ToString());
                pars.Add("photo", linkToPhoto);
                pars.Add("caption", caption);
                webClient.UploadValues(LINK + _token + "/sendPhoto", pars);
            }
        }

        async public Task SendAudioIputFile(int ChatID, string pathToAudio, string catprion = "", int duration = 0, string performer = "", string title = "")
        {
            using (MultipartFormDataContent form = new MultipartFormDataContent())
            {
                string url = LINK + _token + "/sendAudio";
                string fileName = pathToAudio.Split('\\').Last();

                form.Add(new StringContent(ChatID.ToString(), Encoding.UTF8), "chat_id");
                form.Add(new StringContent(catprion.ToString(), Encoding.UTF8), "caption");
                form.Add(new StringContent(duration.ToString(), Encoding.UTF8), "duration");
                form.Add(new StringContent(performer.ToString(), Encoding.UTF8), "performer");
                form.Add(new StringContent(title.ToString(), Encoding.UTF8), "title");
                using (FileStream fileStream = new FileStream(pathToAudio, FileMode.Open, FileAccess.Read))
                {
                    form.Add(new StreamContent(fileStream), "audio", fileName);
                    using (HttpClient client = new HttpClient())
                        await client.PostAsync(url, form);
                }
            }

        }
        public void SendAudioLink(int ChatID, string linkToAudio, string caption = "")
        {
            using (WebClient webClient = new WebClient())
            {
                NameValueCollection pars = new NameValueCollection();
                pars.Add("chat_id", ChatID.ToString());
                pars.Add("audio", linkToAudio);
                pars.Add("caption", caption);
                webClient.UploadValues(LINK + _token + "/sendAudio", pars);
            }
        }

        async public Task SendDocumentIputFile(int ChatID, string pathToDocument, string catprion = "")
        {
            using (MultipartFormDataContent form = new MultipartFormDataContent())
            {
                string url = LINK + _token + "/sendDocument";
                string fileName = pathToDocument.Split('\\').Last();

                form.Add(new StringContent(ChatID.ToString(), Encoding.UTF8), "chat_id");
                form.Add(new StringContent(catprion.ToString(), Encoding.UTF8), "caption");
                using (FileStream fileStream = new FileStream(pathToDocument, FileMode.Open, FileAccess.Read))
                {
                    form.Add(new StreamContent(fileStream), "document", fileName);
                    using (HttpClient client = new HttpClient())
                        await client.PostAsync(url, form);
                }
            }

        }
        public void SendDocumentoLink(int ChatID, string linkToDocument, string caption = "")
        {
            using (WebClient webClient = new WebClient())
            {
                NameValueCollection pars = new NameValueCollection();
                pars.Add("chat_id", ChatID.ToString());
                pars.Add("document", linkToDocument);
                pars.Add("caption", caption);
                webClient.UploadValues(LINK + _token + "/sendDocument", pars);
            }
        }

        public void SendSticker(int chatID, string IDsticker)
        {
            using (WebClient webClient = new WebClient())
            {
                NameValueCollection pars = new NameValueCollection();
                pars.Add("chat_id", chatID.ToString());
                pars.Add("sticker", IDsticker);
                webClient.UploadValues("https://api.telegram.org/bot" + _token + "/sendSticker", pars);
            }
        }

        async public Task SendVideoInputFile(int chatID, string pathToVideo, string caption = "")
        {
            using (var form = new MultipartFormDataContent())
            {
                string url = string.Format("https://api.telegram.org/bot{0}/sendVideo", _token);
                string fileName = pathToVideo.Split('\\').Last();

                form.Add(new StringContent(chatID.ToString(), Encoding.UTF8), "chat_id");
                form.Add(new StringContent(caption, Encoding.UTF8), "caption");

                using (FileStream fileStream = new FileStream(pathToVideo, FileMode.Open, FileAccess.Read))
                {
                    form.Add(new StreamContent(fileStream), "video", fileName);
                    using (var client = new HttpClient())
                    {
                        await client.PostAsync(url, form);
                    }
                }
            }
        }
        public void SendVideoLink(int chatID, string linkToVideo, string caption = "")
        {
            using (WebClient webClient = new WebClient())
            {
                NameValueCollection pars = new NameValueCollection();
                pars.Add("chat_id", chatID.ToString());
                pars.Add("video", linkToVideo);
                pars.Add("caption", caption);
                webClient.UploadValues("https://api.telegram.org/bot" + _token + "/sendVideo", pars);
            }
        }

        async public Task SendVoiceInputFile(int chatID, string pathToVoice, string caption = "", int duration = 0)
        {
            using (MultipartFormDataContent form = new MultipartFormDataContent())
            {
                string url = "https://api.telegram.org/bot" + _token + "/sendVoice";
                string fileName = pathToVoice.Split('\\').Last();

                form.Add(new StringContent(chatID.ToString(), Encoding.UTF8), "chat_id");
                using (FileStream fileStream = new FileStream(pathToVoice, FileMode.Open, FileAccess.Read))
                {
                    form.Add(new StreamContent(fileStream), "voice", fileName);
                    form.Add(new StringContent(caption, Encoding.UTF8), "caption");
                    form.Add(new StringContent(duration.ToString(), Encoding.UTF8), "duration");
                    using (HttpClient client = new HttpClient())
                    {
                        await client.PostAsync(url, form);
                    }
                }
            }
        }
        public void SendVoiceLink(int chatID, string linkToAudio, string caption = "")
        {
            using (WebClient webClient = new WebClient())
            {
                NameValueCollection pars = new NameValueCollection();
                pars.Add("chat_id", chatID.ToString());
                pars.Add("voice", linkToAudio);
                pars.Add("caption", caption);
                webClient.UploadValues("https://api.telegram.org/bot" + _token + "/sendVoice", pars);
            }
        }

        public void SendLocation(int chatID, float latitude, float longitude)
        {
            using (WebClient webClient = new WebClient())
            {
                NameValueCollection pars = new NameValueCollection();
                pars.Add("chat_id", chatID.ToString());
                pars.Add("latitude", latitude.ToString());
                pars.Add("longitude", longitude.ToString());
                webClient.UploadValues("https://api.telegram.org/bot" + _token + "/SendLocation", pars);
            }
        }

        public void SendVenue(int chatID, float latitude, float longitude, string title, string address, string foursquare_id = "1")
        {
            using (WebClient webClient = new WebClient())
            {
                NameValueCollection pars = new NameValueCollection();
                pars.Add("chat_id", chatID.ToString());
                pars.Add("latitude", latitude.ToString());
                pars.Add("longitude", longitude.ToString());
                pars.Add("title", title);
                pars.Add("address", address);
                pars.Add("foursquare_id", foursquare_id);
                webClient.UploadValues("https://api.telegram.org/bot" + _token + "/SendVenue", pars);
            }
        }

        public void SendContact(int chatID, string phone_number, string first_name, string last_name)
        {
            using (WebClient webClient = new WebClient())
            {
                NameValueCollection pars = new NameValueCollection();
                pars.Add("chat_id", chatID.ToString());
                pars.Add("phone_number", phone_number);
                pars.Add("first_name", first_name);
                pars.Add("last_name", last_name);
                webClient.UploadValues("https://api.telegram.org/bot" + _token + "/SendContact", pars);
            }
        }

        public void SendChatAction(int chatID, ChatAction action)
        {
            using (WebClient webClient = new WebClient())
            {
                NameValueCollection pars = new NameValueCollection();
                pars.Add("chat_id", chatID.ToString());
                pars.Add("action", action.ToString());
                webClient.UploadValues("https://api.telegram.org/bot" + _token + "/sendChatAction", pars);
            }
        }
        public enum ChatAction
        {
            typing,
            upload_photo,
            record_video,
            upload_video,
            record_audio,
            upload_audio,
            upload_document,
            find_location
        }

        public string getUserProfilePhotos(int user_id, int offset, int limit = 100)
        {
            using (WebClient webClient = new WebClient())
            {
                string response = webClient.DownloadString(LINK + _token + "/getUserProfilePhotos?user_id=" + user_id + "&limit=" + limit + "&offset=" + offset);
                JSONNode N = JSON.Parse(response);
                N = N["result"]["photos"].AsArray[0];
                string linkPhoto = N[N.Count - 1]["file_id"];
                return linkPhoto;
            }
        }

        public string[] getUserProfilePhotosAllTime(int user_id, int offset, int limit = 100)
        {
            using (WebClient webClient = new WebClient())
            {
                string response = webClient.DownloadString(LINK + _token + "/getUserProfilePhotos?user_id=" + user_id + "&limit=" + limit + "&offset=" + offset);
                JSONNode N = JSON.Parse(response);
                string[] linkPhoto = new string[N["result"]["total_count"].AsInt];
                int k = 0;
                foreach (JSONNode r in N["result"]["photos"].AsArray)
                {
                    linkPhoto[k] = r[r.Count - 1]["file_id"];
                    k++;
                }
                return linkPhoto;
            }
        }

        public string getFile(string file_id)
        {
            using (WebClient webClient = new WebClient())
            {
                string response = webClient.DownloadString(LINK + _token + "/getFile?file_id=" + file_id);
                JSONNode N = JSON.Parse(response);
                response = "https://api.telegram.org/file/bot" + _token + "/" + N["result"]["file_path"];
                return response;
            }
        }
    }
}
