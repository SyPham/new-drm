(window.webpackJsonp=window.webpackJsonp||[]).push([[12],{"1AaR":function(e,t,i){"use strict";i.d(t,"a",function(){return s});var n=i("tk/3"),r=i("AytR"),a=i("fXoL");new n.e({"Content-Type":"application/json",Authorization:"Bearer "+localStorage.getItem("token")});let s=(()=>{class e{constructor(e){this.http=e,this.baseUrl=r.a.apiUrlEC}hasLock(e,t,i){return this.http.get(`${this.baseUrl}Abnormal/HasLock/${e}/${t}/${i}`)}getBatchByIngredientID(e){return this.http.get(`${this.baseUrl}Abnormal/GetBatchByIngredientID/${e}`)}getAll(){return this.http.get(this.baseUrl+"Abnormal/GetAll",{})}create(e){return this.http.post(this.baseUrl+"Abnormal/Create",e)}createRange(e){return this.http.post(this.baseUrl+"Abnormal/CreateRange",e)}update(e){return this.http.put(this.baseUrl+"Abnormal/Update",e)}delete(e){return this.http.delete(this.baseUrl+"Abnormal/Delete/"+e)}getBuildingByIngredientAndBatch(e,t){return this.http.get(`${this.baseUrl}Abnormal/GetBuildingByIngredientAndBatch/${e}/${t}`)}}return e.\u0275fac=function(t){return new(t||e)(a.Zb(n.b))},e.\u0275prov=a.Ib({token:e,factory:e.\u0275fac,providedIn:"root"}),e})()},"5lNp":function(e,t,i){"use strict";i.r(t),i.d(t,"HttpLoaderFactory",function(){return E}),i.d(t,"TroubleshootingModule",function(){return X});var n=i("xxV/"),r=i("ofXK"),a=i("3Pt+"),s=i("JqCM"),l=i("ZOsW"),o=i("jcQU"),c=i("1kSV"),d=i("uF4Q"),h=i("0jTc"),b=i("SRvG"),g=i("ed8r"),u=i("QYJQ"),p=i("tk/3"),m=i("sYmb"),S=i("mqiu"),f=i("Pk9d"),I=i("cZdB"),y=i("GJ+o"),v=i("4SUy"),w=i("hXFv"),R=i("QUNt"),U=i("z+xu"),B=i("ZwAT"),A=i("0hV+"),D=i("tyNb"),C=i("H552"),N=i("fXoL"),x=i("HJFG"),M=i("1AaR"),k=i("h9f+"),G=i("vrRs"),F=i("NkNT");const K=["buildingGrid"];function O(e,t){if(1&e){const e=N.Tb();N.Sb(0,"button",29),N.cc("click",function(){N.Bc(e);const i=t.$implicit;return N.gc().unlock(i)}),N.Nb(1,"i",30),N.Rb()}}let P=(()=>{class e{constructor(e,t,i,n,r,a){this.modalService=e,this.alertify=t,this.abnormalService=i,this.datePipe=n,this.buildingUserService=r,this.ingredientService=a,this.fieldsIngredient={text:"name",value:"id"},this.fieldsBatch={text:"batchName",value:"id"},this.isShow=!1,this.searchSettings={hierarchyMode:"Parent"},this.toolbarOptions=["Search"],this.filterSettings={type:"Excel"},this.ingredients=[],this.pageSettings={pageSize:15},this.abnormal={userID:JSON.parse(localStorage.getItem("user")).user.id,ingredient:"",building:"",batch:""},this.onFilteringIngredientName=e=>{let t=new C.h;t=""!==e.text?t.where("name","contains",e.text,!0):t,e.updateData(this.IngredientData,t)},this.onFilteringBatch=e=>{let t=new C.h;t=""!==e.text?t.where("batchName","contains",e.text,!0):t,e.updateData(this.IngredientData,t)}}ngOnInit(){this.getUsers(),this.getIngredient()}ngAfterViewInit(){}getIngredient(){this.ingredientService.getAllIngredient().subscribe(e=>{this.IngredientData=e})}toolbarClick(e){}actionBegin(e){}actionComplete(e){}openPopupDropdownlist(){$('[data-toggle="tooltip"]').tooltip()}onChangeIngredientName(e){var t,i;e.itemData&&(this.buildings=[],this.batches=[],this.ingredient=null===(t=e.itemData)||void 0===t?void 0:t.name,this.abnormal.ingredient=null===(i=e.itemData)||void 0===i?void 0:i.name,this.abnormalService.getBatchByIngredientID(e.value).subscribe(e=>{this.batches=e}))}onChangeBatch(e){var t,i;e.itemData&&(this.abnormal.batch=null===(t=e.itemData)||void 0===t?void 0:t.batchName,this.abnormalService.getBuildingByIngredientAndBatch(this.ingredient,null===(i=e.itemData)||void 0===i?void 0:i.batchName).subscribe(e=>{this.buildings=e}))}search(){}getUsers(){this.buildingUserService.getAllUsers(1,1e3).subscribe(e=>{const t=e.result.map(e=>({ID:e.ID,Username:e.Username,Email:e.Email}));this.users=t,this.getAll()})}username(e){return this.users.find(t=>t.ID===e).Username}rowSelected(e){this.buildingSelected=this.buildingGrid.getSelectedRecords()}rowDeselected(e){this.buildingSelected=this.buildingGrid.getSelectedRecords()}getAll(){this.abnormalService.getAll().subscribe(e=>{this.abnormals=e.map(e=>({id:e.id,ingredient:e.ingredient,batch:e.batch,building:e.building,lockBy:this.username(e.userID),createdDate:new Date(e.createdDate)}))})}create(){this.abnormalService.create(this.abnormal).subscribe(()=>{this.alertify.success("Successfully!!!"),this.getAll()})}createRange(){if(!this.buildingSelected)return void this.alertify.warning("Please chose buildings first!");const e=this.buildingSelected.map(e=>({building:e.name,ingredient:this.abnormal.ingredient,userID:this.abnormal.userID,batch:this.abnormal.batch}));this.abnormalService.createRange(e).subscribe(()=>{this.alertify.success("Successfully!!!"),this.getAll()})}delete(e){this.abnormalService.delete(e).subscribe(()=>{this.alertify.success("Successfully!!!"),this.getAll()})}lock(){this.createRange()}unlock(e){this.delete(e.id)}}return e.\u0275fac=function(t){return new(t||e)(N.Mb(c.b),N.Mb(x.a),N.Mb(M.a),N.Mb(r.d),N.Mb(k.a),N.Mb(G.a))},e.\u0275cmp=N.Gb({type:e,selectors:[["app-abnormal-list"]],viewQuery:function(e,t){if(1&e&&N.Qc(K,!0),2&e){let e;N.xc(e=N.dc())&&(t.buildingGrid=e.first)}},features:[N.yb([r.d])],decls:40,vars:18,consts:[[1,"row"],[1,"col-md-4"],[1,"card"],[1,"card-body"],[1,"col-md-6"],["placeholder","Select a Ingredient Name",1,"w-100","float-left",3,"dataSource","fields","allowFiltering","ngModel","ngModelChange","filtering","change"],["ingredientDropdownlist",""],["placeholder","Select a batch",1,"w-100","float-left",3,"dataSource","fields","allowFiltering","ngModel","ngModelChange","filtering","change"],["batchDropdownlist",""],[1,"row","mt-3"],[1,"col-md-12"],["rowHeight","38","height","300",3,"dataSource","allowSelection","allowSorting","rowDeselected","rowSelected"],["buildingGrid",""],["type","checkbox","isPrimaryKey","true","width","60"],["field","name","isPrimaryKey","true","headerText","Building","width","130"],[1,"col-md-12","text-center"],["appPermission","","appFunction","Black List","appAction","LOCK",1,"btn","btn-info",3,"click"],[1,"fa","fa-lock"],[1,"col-md-8"],[1,"card-header"],["rowHeight","38","showColumnMenu","true","value","Menu","height","300",3,"dataSource","allowSelection","allowFiltering","toolbar","filterSettings","allowSorting","rowDeselected","rowSelected"],["abnormalGrid",""],["field","ingredient","isPrimaryKey","true","textAlign","Center","headerText","Ingredient","width","130"],["field","batch","isPrimaryKey","true","textAlign","Center","headerText","Batch","width","90"],["field","building","isPrimaryKey","true","textAlign","Center","headerText","Building","width","90"],["field","lockBy","isPrimaryKey","true","textAlign","Center","headerText","Lock By","width","130"],["field","createdDate","isPrimaryKey","true","textAlign","Center","type","dateTime","format","d MMM, yyyy HH:mm","headerText","Lock Date","width","190"],["field","","headerText","Option","textAlign","Center","width","80"],["template",""],["appPermission","","appFunction","Black List","appAction","UNLOCK","type","button",1,"btn","btn-xs","btn-danger",3,"click"],[1,"fa","fa-unlock"]],template:function(e,t){1&e&&(N.Sb(0,"div",0),N.Sb(1,"div",1),N.Sb(2,"div",2),N.Sb(3,"div",3),N.Sb(4,"div",0),N.Sb(5,"div",4),N.Sb(6,"ejs-dropdownlist",5,6),N.cc("ngModelChange",function(e){return t.ingredientID=e})("filtering",function(e){return t.onFilteringIngredientName(e)})("change",function(e){return t.onChangeIngredientName(e)}),N.Rb(),N.Rb(),N.Sb(8,"div",4),N.Sb(9,"ejs-dropdownlist",7,8),N.cc("ngModelChange",function(e){return t.ingredientID=e})("filtering",function(e){return t.onFilteringBatch(e)})("change",function(e){return t.onChangeBatch(e)}),N.Rb(),N.Rb(),N.Rb(),N.Sb(11,"div",9),N.Sb(12,"div",10),N.Sb(13,"ejs-grid",11,12),N.cc("rowDeselected",function(e){return t.rowDeselected(e)})("rowDeselected",function(e){return t.rowDeselected(e)})("rowSelected",function(e){return t.rowSelected(e)}),N.Sb(15,"e-columns"),N.Nb(16,"e-column",13),N.Nb(17,"e-column",14),N.Rb(),N.Rb(),N.Rb(),N.Rb(),N.Sb(18,"div",9),N.Sb(19,"div",15),N.Sb(20,"button",16),N.cc("click",function(){return t.lock()}),N.Nb(21,"i",17),N.Kc(22," Lock"),N.Rb(),N.Rb(),N.Rb(),N.Rb(),N.Rb(),N.Rb(),N.Sb(23,"div",18),N.Sb(24,"div",2),N.Sb(25,"div",19),N.Sb(26,"h3"),N.Kc(27,"Black List"),N.Rb(),N.Rb(),N.Sb(28,"div",3),N.Sb(29,"ejs-grid",20,21),N.cc("rowDeselected",function(e){return t.rowDeselected(e)})("rowDeselected",function(e){return t.rowDeselected(e)})("rowSelected",function(e){return t.rowSelected(e)}),N.Sb(31,"e-columns"),N.Nb(32,"e-column",22),N.Nb(33,"e-column",23),N.Nb(34,"e-column",24),N.Nb(35,"e-column",25),N.Nb(36,"e-column",26),N.Sb(37,"e-column",27),N.Ic(38,O,2,0,"ng-template",null,28,N.Jc),N.Rb(),N.Rb(),N.Rb(),N.Rb(),N.Rb(),N.Rb(),N.Rb()),2&e&&(N.zb(6),N.mc("dataSource",t.IngredientData)("fields",t.fieldsIngredient)("allowFiltering",!0)("ngModel",t.ingredientID),N.zb(3),N.mc("dataSource",t.batches)("fields",t.fieldsBatch)("allowFiltering",!0)("ngModel",t.ingredientID),N.zb(4),N.mc("dataSource",t.buildings)("allowSelection",!0)("allowSorting",!0)("allowSorting",!0),N.zb(16),N.mc("dataSource",t.abnormals)("allowSelection",!0)("allowFiltering",!0)("toolbar",t.toolbarOptions)("filterSettings",t.filterSettings)("allowSorting",!0))},directives:[o.c,a.g,a.j,y.j,h.d,R.d,R.b,y.e,y.b,h.c,R.c,R.a,y.d,y.a,F.a],styles:[""]}),e})();var L=i("0WEV");function T(e,t){if(1&e){const e=N.Tb();N.Sb(0,"button",18),N.cc("click",function(){N.Bc(e);const i=t.$implicit;return N.gc(2).searchBatch(i)}),N.Kc(1),N.Rb()}if(2&e){const e=t.$implicit;N.zb(1),N.Lc(e.batchName)}}function z(e,t){if(1&e&&(N.Sb(0,"div",11),N.Ic(1,T,2,1,"button",17),N.Rb()),2&e){const e=N.gc();N.zb(1),N.mc("ngForOf",e.dataBatch)}}function j(e,t){if(1&e&&(N.Sb(0,"tr"),N.Sb(1,"th",23),N.Kc(2),N.Rb(),N.Sb(3,"td",24),N.Kc(4),N.Rb(),N.Sb(5,"td",24),N.Kc(6),N.Rb(),N.Sb(7,"td",24),N.Kc(8),N.Rb(),N.Sb(9,"td",24),N.Kc(10),N.Rb(),N.Sb(11,"td",24),N.Kc(12),N.Rb(),N.Sb(13,"td",24),N.Kc(14),N.Rb(),N.Sb(15,"td",24),N.Kc(16),N.hc(17,"date"),N.Rb(),N.Rb()),2&e){const e=t.$implicit;N.zb(2),N.Lc(e.ingredient),N.zb(2),N.Lc(e.batch),N.zb(2),N.Lc(e.modelName),N.zb(2),N.Lc(e.modelNo),N.zb(2),N.Lc(e.articleNo),N.zb(2),N.Lc(e.process),N.zb(2),N.Mc("",e.line," "),N.zb(2),N.Mc("",N.jc(17,8,e.mixDate,"yyyy-MM-dd , hh:mm:ss a")," ")}}function Q(e,t){if(1&e&&(N.Sb(0,"div",3),N.Sb(1,"div",4),N.Sb(2,"div",5),N.Sb(3,"table",19),N.Sb(4,"thead",20),N.Sb(5,"tr"),N.Sb(6,"th",21),N.Kc(7,"Ingredient"),N.Rb(),N.Sb(8,"th",21),N.Kc(9,"Batch"),N.Rb(),N.Sb(10,"th",21),N.Kc(11,"Model Name"),N.Rb(),N.Sb(12,"th",21),N.Kc(13,"Model No."),N.Rb(),N.Sb(14,"th",21),N.Kc(15,"Article No."),N.Rb(),N.Sb(16,"th",21),N.Kc(17,"Process"),N.Rb(),N.Sb(18,"th",21),N.Kc(19,"Line"),N.Rb(),N.Sb(20,"th",21),N.Kc(21,"Mix Date"),N.Rb(),N.Rb(),N.Rb(),N.Sb(22,"tbody"),N.Ic(23,j,18,11,"tr",22),N.Rb(),N.Rb(),N.Rb(),N.Rb(),N.Rb()),2&e){const e=N.gc();N.zb(23),N.mc("ngForOf",e.dataSearch)}}const J=[{path:"",data:{title:"Troubleshooting",breadcrumb:"Troubleshooting"},children:[{path:"search",component:(()=>{class e{constructor(e,t,i,n,r,a){this.modalService=e,this.alertify=t,this.datePipe=i,this.ingredientService=n,this.planService=r,this.spinner=a,this.fieldsIngredient={text:"name",value:"id"},this.isShow=!1,this.showBatch=!1,this.ingredients=[],this.onFilteringIngredientName=e=>{let t=new C.h;t=""!==e.text?t.where("name","contains",e.text,!0):t,e.updateData(this.IngredientData,t)}}ngOnInit(){this.getIngredient()}getIngredient(){this.ingredientService.getAllIngredient().subscribe(e=>{this.IngredientData=e})}openPopupDropdownlist(){$('[data-toggle="tooltip"]').tooltip()}onChangeIngredientName(e){this.isShow=!1,this.showBatch=!0;const t=e.value;this.ingredientName=e.itemData.name,this.GetBatchByIngredientID(t)}GetBatchByIngredientID(e){this.planService.GetBatchByIngredientID(e).subscribe(e=>{this.dataBatch=e})}searchBatch(e){this.isShow=!0,this.spinner.show(),this.planService.TroubleShootingSearch(this.ingredientName,e.batchName).subscribe(e=>{this.dataSearch=e,this.spinner.hide()})}}return e.\u0275fac=function(t){return new(t||e)(N.Mb(c.b),N.Mb(x.a),N.Mb(r.d),N.Mb(G.a),N.Mb(L.a),N.Mb(s.c))},e.\u0275cmp=N.Gb({type:e,selectors:[["app-search"]],features:[N.yb([r.d])],decls:32,vars:7,consts:[["type","square-jelly-box","size","medium",3,"fullScreen"],[1,"loading"],[1,"card"],[1,"card-body"],[1,"row"],[1,"col-md-12"],[1,"col-sm-12","text-center"],["for","",1,"font-weight-bold",2,"color","transparent"],[1,"col-md-5"],[1,"col-md-12","card-body","table-responsive"],[1,"col-md-3","mt-2"],[1,"col-md-8","mt-2"],["placeholder","Select a Ingredient Name",1,"w-100","float-left",3,"dataSource","fields","allowFiltering","ngModel","ngModelChange","filtering","change"],["ingredientDropdownlist",""],[1,"col-md-7"],["class","col-md-8 mt-2",4,"ngIf"],["class","card-body",4,"ngIf"],["style","margin: 3px;","type","button","class","btn btn-info btn-sm",3,"click",4,"ngFor","ngForOf"],["type","button",1,"btn","btn-info","btn-sm",2,"margin","3px",3,"click"],[1,"table","table-bordered"],[1,"thead-dark"],["scope","col",2,"text-align","center"],[4,"ngFor","ngForOf"],["scope","row",2,"text-align","center"],[2,"text-align","center"]],template:function(e,t){1&e&&(N.Sb(0,"ngx-spinner",0),N.Sb(1,"p",1),N.Kc(2,"Loading data..."),N.Rb(),N.Rb(),N.Sb(3,"div",2),N.Sb(4,"div",3),N.Sb(5,"div",4),N.Sb(6,"div",5),N.Sb(7,"div",4),N.Sb(8,"div",6),N.Sb(9,"label",7),N.Kc(10,"Scan QRCode"),N.Rb(),N.Rb(),N.Rb(),N.Qb(11),N.Sb(12,"div",4),N.Sb(13,"div",8),N.Sb(14,"div",2),N.Sb(15,"div",9),N.Sb(16,"div",4),N.Sb(17,"div",10),N.Sb(18,"label"),N.Kc(19,"Search Ingredient: "),N.Rb(),N.Rb(),N.Sb(20,"div",11),N.Sb(21,"ejs-dropdownlist",12,13),N.cc("ngModelChange",function(e){return t.ingredientID=e})("filtering",function(e){return t.onFilteringIngredientName(e)})("change",function(e){return t.onChangeIngredientName(e)}),N.Rb(),N.Rb(),N.Rb(),N.Rb(),N.Rb(),N.Rb(),N.Sb(23,"div",14),N.Sb(24,"div",2),N.Sb(25,"div",9),N.Sb(26,"div",4),N.Sb(27,"div",10),N.Sb(28,"label"),N.Kc(29,"Ingredient Batch: "),N.Rb(),N.Rb(),N.Ic(30,z,2,1,"div",15),N.Rb(),N.Rb(),N.Rb(),N.Rb(),N.Rb(),N.Pb(),N.Rb(),N.Rb(),N.Rb(),N.Ic(31,Q,24,1,"div",16),N.Rb()),2&e&&(N.mc("fullScreen",!1),N.zb(21),N.mc("dataSource",t.IngredientData)("fields",t.fieldsIngredient)("allowFiltering",!0)("ngModel",t.ingredientID),N.zb(9),N.mc("ngIf",t.showBatch),N.zb(1),N.mc("ngIf",t.isShow))},directives:[s.a,o.c,a.g,a.j,r.m,r.l],pipes:[r.d],styles:[""]}),e})(),data:{title:"Troubleshooting Search",breadcrumb:"Search"}},{path:"Abnormal-List",component:P,data:{title:"Troubleshooting Black List",breadcrumb:"Abnormal-List"}}]}];let V,H=(()=>{class e{}return e.\u0275mod=N.Kb({type:e}),e.\u0275inj=N.Jb({factory:function(t){return new(t||e)},imports:[[D.g.forChild(J)],D.g]}),e})();function E(e){return new S.a(e,"./assets/i18n/",".json")}const W=localStorage.getItem("lang");Object(v.bb)(i("2PVe"),i("GFvd"),i("GFWS"),i("0zpJ"),i("LRxf")),Object(v.bb)(i("2PVe"),i("oP7I"),i("Joj5"),i("3JeS"),i("LRxf")),V="vi"===W?W:"en";let X=(()=>{class e{constructor(){"vi"===W?(V="vi",setTimeout(()=>{v.p.load(i("+7Wi")),Object(v.nb)("vi")})):(V="en",setTimeout(()=>{v.p.load(i("oyrQ")),Object(v.nb)("en")}))}}return e.\u0275mod=N.Kb({type:e}),e.\u0275inj=N.Jb({factory:function(t){return new(t||e)},providers:[r.d],imports:[[A.b,b.c,r.b,a.c,a.m,s.b,H,l.a,o.d,c.c,h.b,h.a,h.e,d.a,d.c,d.b,b.j,u.a,g.b,R.h,y.i,b.h,f.c,B.b,I.a,g.d,U.a,w.d,b.f,o.f,n.a,m.b.forChild({loader:{provide:m.a,useFactory:E,deps:[p.b]},defaultLanguage:V})]]}),e})()},NkNT:function(e,t,i){"use strict";i.d(t,"a",function(){return s});var n=i("fXoL"),r=i("tyNb"),a=i("x+4Q");let s=(()=>{class e{constructor(e,t,i){this.el=e,this.route=t,this.authService=i}ngOnInit(){if(this.authService.loggedIn()){const e=JSON.parse(localStorage.getItem("functions")).map(e=>e.childrens),t=this.route.snapshot.data.functionCode,i=[].concat.apply([],e).filter(e=>e.functionCode===t);this.el.nativeElement.style.display=i&&i.filter(e=>e.functionCode===this.appFunction&&e.code===this.appAction).length>0?"":"none"}else this.el.nativeElement.style.display="none"}}return e.\u0275fac=function(t){return new(t||e)(n.Mb(n.l),n.Mb(r.a),n.Mb(a.a))},e.\u0275dir=n.Hb({type:e,selectors:[["","appPermission",""]],inputs:{appFunction:"appFunction",appAction:"appAction"}}),e})()},"h9f+":function(e,t,i){"use strict";i.d(t,"a",function(){return c});var n=i("2Vo4"),r=i("lJxs"),a=i("tk/3"),s=i("AytR"),l=i("2zK7"),o=i("fXoL");new a.e({"Content-Type":"application/json",Authorization:"Bearer "+localStorage.getItem("token")});let c=(()=>{class e{constructor(e){this.http=e,this.baseUrl=s.a.apiUrlEC,this.authUrl=s.a.apiUrl,this.messageSource=new n.a(0),this.currentMessage=this.messageSource.asObservable()}changeMessage(e){this.messageSource.next(e)}getBuildingsAsTreeView(){return this.http.get(this.baseUrl+"Building/GetAllAsTreeView")}mappingUserWithBuilding(e){return this.http.post(this.baseUrl+"BuildingUser/MappingUserWithBuilding",e)}removeBuildingUser(e){return this.http.post(this.baseUrl+"BuildingUser/RemoveBuildingUser",e)}getAllUsers(e,t){const i=new l.a;return this.http.get(`${this.authUrl}Users/GetAllUsers/${s.a.systemCode}/${e}/${t}`,{observe:"response"}).pipe(Object(r.a)(e=>(i.result=e.body,null!=e.headers.get("Pagination")&&(i.pagination=JSON.parse(e.headers.get("Pagination"))),i)))}deleteUser(e){return this.http.delete(`${this.authUrl}Users/Delete/${e}`)}updateUser(e){return this.http.post(this.authUrl+"Users/Update",e)}createUser(e){return this.http.post(this.authUrl+"Users/Create",e)}getAllBuildingUsers(){return this.http.get(this.baseUrl+"BuildingUser/getAllBuildingUsers")}getBuildingUserByBuildingID(e){return this.http.get(`${this.baseUrl}BuildingUser/GetBuildingUserByBuildingID/${e}`)}}return e.\u0275fac=function(t){return new(t||e)(o.Zb(a.b))},e.\u0275prov=o.Ib({token:e,factory:e.\u0275fac,providedIn:"root"}),e})()},vrRs:function(e,t,i){"use strict";i.d(t,"a",function(){return c});var n=i("tk/3"),r=i("2Vo4"),a=i("lJxs"),s=i("AytR"),l=i("2zK7"),o=i("fXoL");new n.e({"Content-Type":"application/json"});let c=(()=>{class e{constructor(e){this.http=e,this.baseUrl=s.a.apiUrlEC,this.ingredientSource=new r.a(0),this.currentIngredient=this.ingredientSource.asObservable()}getIngredients(e,t){const i=new l.a;let r=new n.f;return null!=e&&null!=t&&(r=r.append("pageNumber",e),r=r.append("pageSize",t)),this.http.get(this.baseUrl+"ingredient/getingredients",{observe:"response",params:r}).pipe(Object(a.a)(e=>(i.result=e.body,null!=e.headers.get("Pagination")&&(i.pagination=JSON.parse(e.headers.get("Pagination"))),i)))}search(e,t,i){const r=new l.a;let s=new n.f;return null!=e&&null!=t&&(s=s.append("pageNumber",e),s=s.append("pageSize",t)),this.http.get(this.baseUrl+"ingredient/Search/"+i,{observe:"response",params:s}).pipe(Object(a.a)(e=>(r.result=e.body,null!=e.headers.get("Pagination")&&(r.pagination=JSON.parse(e.headers.get("Pagination"))),r)))}getAllIngredient(){return this.http.get(this.baseUrl+"ingredient/GetAll",{})}rate(){return this.http.get(this.baseUrl+"ingredient/Rate",{})}getAllGlueType(){return this.http.get(this.baseUrl+"ingredient/GetAllGlueType",{})}getByID(e){return this.http.get(this.baseUrl+"ingredient/GetbyID/"+e,{})}scanQRCode(e){return this.http.get(this.baseUrl+"ingredient/ScanQRCode/"+e,{})}scanQRCodeFromChemicalWareHouse(e,t,i){return this.http.post(`${this.baseUrl}ingredient/ScanQRCodeFromChemialWareHouse/${e}/${t}/${i}`,{})}scanQRCodeFromChemicalWareHouseV1(e){return this.http.post(this.baseUrl+"ingredient/ScanQRCodeFromChemialWareHouseV1/",e)}scanQRCodeOutputV1(e){return this.http.post(this.baseUrl+"ingredient/ScanQRCodeOutputV1/",e)}scanQRCodeOutput(e,t,i){return this.http.get(`${this.baseUrl}ingredient/ScanQRCodeOutput/${e}/${t}/${i}`,{})}getAllSupplier(){return this.http.get(this.baseUrl+"Suppier/GetAll",{})}getAllIngredientInfo(){return this.http.get(this.baseUrl+"ingredient/GetAllIngredientInfo",{})}getAllIngredientInfoOutput(){return this.http.get(this.baseUrl+"ingredient/GetAllIngredientInfoOutPut",{})}getAllIngredientInfoByBuildingName(e){return this.http.get(this.baseUrl+"ingredient/GetAllIngredientInfoByBuildingName/"+e,{})}getAllIngredientInfoReport(){return this.http.get(this.baseUrl+"ingredient/GetAllIngredientInfoReport",{})}getAllIngredientInfoReportByBuildingName(e){return this.http.get(this.baseUrl+"ingredient/GetAllIngredientInfoReportByBuildingName/"+e,{})}searchInventory(e,t){return this.http.get(`${this.baseUrl}ingredient/Search/${e}/${t}`,{})}searchInventoryByBuildingName(e,t,i){return this.http.get(`${this.baseUrl}ingredient/SearchWithBuildingName/${e}/${t}/${i}`,{})}deleteIngredientInfo(e,t,i,n){return this.http.delete(this.baseUrl+`ingredient/DeleteIngredientInfo/${e}/${t}/${i}/${n}`)}UpdateConsumption(e,t,i){return this.http.post(this.baseUrl+`ingredient/UpdateConsumptionIngredientReport/${e}/${t}/${i}`,{})}UpdateConsumptionOfBuilding(e){return this.http.post(this.baseUrl+"ingredient/UpdateConsumptionOfBuildingIngredientReport",e)}createSub(e){return this.http.post(this.baseUrl+"suppier/create",e)}updateSub(e){return this.http.put(this.baseUrl+"suppier/update",e)}deleteSub(e){return this.http.delete(this.baseUrl+"suppier/delete/"+e)}sortBySup(e,t){return this.http.get(this.baseUrl+`GlueIngredient/GetIngredientsByGlueID/${e}/${t}`)}getIngredientsByGlueID(e){return this.http.get(this.baseUrl+"GlueIngredient/GetIngredientsByGlueID/"+e)}create(e){return this.http.post(this.baseUrl+"ingredient/create1",e)}update(e){return this.http.put(this.baseUrl+"ingredient/update",e)}delete(e){return this.http.delete(this.baseUrl+"ingredient/delete/"+e)}changeIngredient(e){this.ingredientSource.next(e)}import(e,t){const i=new FormData;return i.append("UploadedFile",e),i.append("CreatedBy",t),this.http.post(this.baseUrl+"ingredient/Import",i)}downloadFile(){return this.http.post(this.baseUrl+"ingredient/ExcelExport",{})}GetQrcodeByid(e){return this.http.get(this.baseUrl+"ingredient/GetbyID/"+e)}UpdatePrint(e){return this.http.put(this.baseUrl+"ingredient/UpdatePrint",e)}checkIncoming(e,t,i){return this.http.get(`${this.baseUrl}ingredient/CheckIncoming/${e}/${t}/${i}`,{})}getAllIngredientInfoByBuilding(e){return this.http.get(this.baseUrl+"ingredient/GetAllIngredientInfoByBuilding/"+e,{})}getAllIngredientInfoOutputByBuilding(e){return this.http.get(this.baseUrl+"ingredient/GetAllIngredientInfoOutputByBuilding/"+e,{})}}return e.\u0275fac=function(t){return new(t||e)(o.Zb(n.b))},e.\u0275prov=o.Ib({token:e,factory:e.\u0275fac,providedIn:"root"}),e})()}}]);