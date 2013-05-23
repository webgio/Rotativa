This folder contains files necessary to Rotativa to generate PDF files.

Please don't move or rename them nor the folder itself.

If you really need to, you should set the WkhtmltopdfPath property on that 
folder path when creating a new ViewAsPdf, PartialViewAsPdf, ActionAsPdf, 
RouteAsPdf, UrlAsPdf. 

Example: 

public ActionResult MyControllerAction
{
    // Your MVC controller action
    // ...
    
    string rotativaLibsPath = HostingEnvironment.MapPath("~/MyLibs/RotativaLibs");
    
    return new ViewAsPdf("myViewName", myViewModel) { WkhtmltopdfPath = rotativaLibsPath };
}
