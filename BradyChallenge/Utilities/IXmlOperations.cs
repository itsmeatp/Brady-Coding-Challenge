using System;

namespace BradyChallenge.Utilities
{
    interface IXmlOperations
    {
        object FromXml(string Xml, Type ObjType);
        string ExtractInputData(string file);
        string ToXml(object Obj, Type ObjType);
    }
}
