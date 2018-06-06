using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;


namespace HUST_OutPut
{
    class XmlNodeTest
    {
        public static string TestAttributes()
        {
            XmlDocument doc = new XmlDocument();
            doc.Load("test.xml");

            XmlNode root = doc.DocumentElement;

            //Create a new attribute. 
           
            //Add the attribute to the document.
            XmlNode child = root.ChildNodes[0];
            if (child.Attributes["myname"] != null)
                return "yes";
            else
                return "no"; ;

        }
    }
}
