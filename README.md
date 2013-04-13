# NTestData


### Overview
Generate POCOs from your DB for easy testing.

I like to begin a project by doing testing integrated with my test DB; so my repositories are at that point actually pointing at the db. Generally after that i kept a "test database" with my specific test data.

NTestData seeks to kill that db-- instead you just run NTestData on your database and it spits out POCOs you can base an IRepository on.

### Special Thanks (Projects this is hacked upon)
* [ServiceStack.OrmLite](https://github.com/ServiceStack/ServiceStack.OrmLite)'s OrmLite.tt file. Since my models are based upon that .tt it only made sense to reuse part of its functionality. (Copyright (c) 2007-2011, Demis Bellot, ServiceStack.)
* [Dapper](https://code.google.com/p/dapper-dot-net/). The secret to making a great db tool is making `Dapper.SqlMapper.DapperRow` `public`. Now you know. (Copyright http://www.apache.org/licenses/LICENSE-2.0)
* [CuttingEdge.Conditions](http://conditions.codeplex.com/) For gaurd-ish stuff (MIT)

### License
As free as the above licenses allow it to be. 
