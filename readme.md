## SQL Server Versions API and Website

###Description

The API serves as a way to consume SQL Server version information on builds, supportability, and other surrounding data.  The website is the accompanying user interface for searching version data and other relevant operations.

###API Reference

**Version Search**

*Get all versions [GET]*
http://sqlserverversions.azurewebsites.net/api/version

*Get a single version [GET]*
http://sqlserverversions.azurewebsites.net/api/version/{major}/{minor}/{build}/{revision}

**Recent Versions and Supportability**

*Get the 5 most recent versions [GET]*
http://sqlserverversions.azurewebsites.net/api/recent

*Get a specified amount of recent versions [GET]*
http://sqlserverversions.azurewebsites.net/api/recent/{topcount}

*Get the most recent by major release [GET]*
http://sqlserverversions.azurewebsites.net/api/latest/{major}/{minor}

*Get the oldest supported versions by major release [GET]*
http://sqlserverversions.azurewebsites.net/api/oldestsupported/{major}/{minor}
