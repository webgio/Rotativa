#Extremely easy way to create Pdf files from Asp.net MVC.

##Usage:

```csharp
public ActionResult PrintIndex()
{
    return new ActionAsPdf("Index", new { name = "Giorgio" }) { FileName = "Test.pdf" };
}

public ActionResult Index(string name)
{
    ViewBag.Message = string.Format("Hello {0} to ASP.NET MVC!", name);

    return View();
}
```
ViewAsPdf now available. It enables you to render a view as pdf in just one move, thanks to scoorf
```csharp
public ActionResult TestViewWithModel(string id)
{
    var model = new TestViewModel {DocTitle = id, DocContent = "This is a test"};
    return new ViewAsPdf(model);
}
```
Also available a RouteAsPdf, UrlAsPdf and ViewAsPdf ActionResult.

It generates Pdf also from authorized actions (web forms authentication).

You can also output images from MVC with ActionAsImage, ViewAsImage, RouteAsImage, UrlAsImage.

##Rotativa HQ

[RotativaHQ.com](http://rotativahq.com) is an API (SaaS) version of Rotativa, hosted on Azure. You can register for free.
