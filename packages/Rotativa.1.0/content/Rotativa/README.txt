This folder contains file necessary to Rotativa to generate Pdf files

Please don't move or rename them or the folder itself

If you need to you should add a key in web.config with the path to this files:

  <appSettings>
	
    <add key="WkhtmltopdfPath" value="c:\pathtothefolder"/>

  </appSettings>