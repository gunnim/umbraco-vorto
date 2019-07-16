namespace Our.Umbraco.Vorto.Web.PackageActions
{
    //public class DeleteFilePackageAction : IPackageAction
    //{
    //	public string Alias()
    //	{
    //		return "Vorto_DeleteFile";
    //	}

    //	public bool Execute(string packageName, XElement xmlData)
    //	{
    //		var node = xmlData.SelectSingleNode("//Action[@alias='" + Alias() + "']");
    //		var filePath = node.Attributes["path"].Value;
    //		var absoluteFilePath = HttpContext.Current.Server.MapPath(filePath);

    //		if(File.Exists(absoluteFilePath))
    //			File.Delete(absoluteFilePath);

    //		return true;
    //	}

    //	public bool Undo(string packageName, XElement xmlData)
    //	{
    //		return true;
    //	}

    //	public XmlNode SampleXml()
    //	{
    //		return helper.parseStringToXmlNode("<Action runat=\"install\" undo=\"true/false\" alias=\"Vorto_DeleteFile\" path=\"path\" />");
    //	}
    //}
}
