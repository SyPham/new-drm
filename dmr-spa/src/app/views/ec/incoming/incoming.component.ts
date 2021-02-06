import { Component, OnInit, AfterViewInit, ViewChild, Renderer2, ElementRef, QueryList, HostListener, OnDestroy, ChangeDetectorRef } from '@angular/core';
import { NgbModal } from '@ng-bootstrap/ng-bootstrap';
import { AlertifyService } from 'src/app/_core/_service/alertify.service';
import { DisplayTextModel } from '@syncfusion/ej2-angular-barcode-generator';
import { IngredientService } from 'src/app/_core/_service/ingredient.service';
import { DatePipe } from '@angular/common';
import { GridComponent } from '@syncfusion/ej2-angular-grids';
import { Subject, Subscription } from 'rxjs';
import { IScanner } from 'src/app/_core/_model/IToDoList';
import { debounceTime } from 'rxjs/operators';
import { IBuilding } from 'src/app/_core/_model/building';

import { DropDownListComponent, FilteringEventArgs } from '@syncfusion/ej2-angular-dropdowns';
import { Query } from '@syncfusion/ej2-data/';
import { EmitType } from '@syncfusion/ej2-base';
import { BuildingService } from 'src/app/_core/_service/building.service';
const BUILDING_LEVEL = 2;
@Component({
  selector: 'app-incoming',
  templateUrl: './incoming.component.html',
  styleUrls: ['./incoming.component.css'],
  providers: [
    DatePipe
  ]
})
export class IncomingComponent implements OnInit, OnDestroy, AfterViewInit {
  @ViewChild('ddlelement')
  public dropDownListObject: DropDownListComponent;
  @ViewChild('scanQRCode') scanQRCodeElement: ElementRef;
  public displayTextMethod: DisplayTextModel = {
    visibility: false
  };
  // public filterSettings: object;
  pageSettings = { pageCount: 20, pageSizes: true, pageSize: 20 };
  @ViewChild('grid') public grid: GridComponent;
  // toolbarOptions: string[];
  @ViewChild('scanText', { static: false }) scanText: ElementRef;
  @ViewChild('ingredientinfoGrid') ingredientinfoGrid: GridComponent;
  qrcodeChange: any;
  data: [];
  dataOut: [];
  checkout = false;
  checkin = true;
  public ingredients: any = [];
  test: any = 'form-control w3-light-grey';
  checkCode: boolean;
  autofocus = false;
  toolbarOptions = ['Search'];
  filterSettings = { type: 'Excel' };
  subject = new Subject<IScanner>();
  subscription: Subscription[] = [];

  buildings: IBuilding[];
  fieldsBuildings: object = { text: 'name', value: 'id' };
  buildingID = 0;
  buildingName = '';
  constructor(
    public modalService: NgbModal,
    private alertify: AlertifyService,
    private datePipe: DatePipe,
    private buildingService: BuildingService,
    public ingredientService: IngredientService,
    private cdr: ChangeDetectorRef
  ) {
  }
  ngAfterViewInit(): void {
    this.cdr.detectChanges();
  }
  ngOnDestroy(): void {
    this.subscription.forEach(item => item.unsubscribe());
  }
  public ngOnInit(): void {
    // this.getIngredientInfo();
    this.getAllIngredient();
    this.checkQRCode();
    this.getBuilding(() => {
      this.buildingID = +localStorage.getItem('buildingID');
    });
  }
  getBuilding(callback): void {
    this.buildingService.getBuildings().subscribe(async (buildingData) => {
      this.buildings = buildingData.filter(item => item.level === BUILDING_LEVEL);
      callback();
    });
  }
  onFilteringBuilding: EmitType<FilteringEventArgs> = (
    e: FilteringEventArgs
  ) => {
    let query: Query = new Query();
    // frame the query based on search string with filter type.
    query =
      e.text !== '' ? query.where('name', 'contains', e.text, true) : query;
    // pass the filter data source, filter query to updateData method.
    e.updateData(this.buildings as any, query);
  }
  onChangeBuilding(args) {
    this.buildingID = args.itemData.id;
    this.buildingName = args.itemData.name;
    this.getAllIngredientInfoByBuilding();
  }
  onSelectBuilding(args: any): void {
    this.buildingID = args.itemData.id;
    this.buildingName = args.itemData.name;
    this.getAllIngredientInfoByBuilding();
  }
  NO(index) {
    return (this.ingredientinfoGrid.pageSettings.currentPage - 1) * this.ingredientinfoGrid.pageSettings.pageSize + Number(index) + 1;
  }
  dataBound() {
    this.ingredientinfoGrid.autoFitColumns();
  }
  OutputChange(args) {
    this.checkin = false;
    this.checkout = true;
    // this.qrcodeChange = null ;
    this.getAllIngredientInfoOutputByBuilding();
  }

  InputChange(args) {
    this.checkin = true;
    this.checkout = false;
    this.getAllIngredientInfoByBuilding();
    // this.qrcodeChange = null ;
  }
  toolbarClick(args): void {
    switch (args.item.text) {
      /* tslint:disable */
      case 'Excel Export':
        this.grid.excelExport();
        break;
      /* tslint:enable */
      case 'PDF Export':
        break;
    }
  }
  private checkQRCode() {
    this.subscription.push(this.subject
      .pipe(debounceTime(500))
      .subscribe(async (res) => {
        const input = res.QRCode.split('-') || [];
        // const commonPattern = /(\d+)-(\w+)-([\w\-\d]+)/g;
        const dateAndBatch = /(\d+)-(\w+)-/g;
        const validFormat = res.QRCode.match(dateAndBatch);
        const qrcode = res.QRCode.replace(validFormat[0], '');
        const levels = [1, 0];
        const building = JSON.parse(localStorage.getItem('building'));
        let buildingName = building.name;
        if (levels.includes(building.level)) {
          buildingName = 'E';
        }
        this.findIngredientCode(qrcode);
        if (this.checkin === true) {
          if (this.checkCode === true) {
            const userID = JSON.parse(localStorage.getItem('user')).User.ID;
            this.ingredientService.scanQRCodeFromChemicalWareHouse(res.QRCode, this.buildingName, userID).subscribe((status: any) => {
              if (status === true) {
                this.getAllIngredientInfoByBuilding();
              }
            });
          } else {
            this.alertify.error('Wrong Chemical!');
          }
        } else {
          if (this.checkCode === true) {
            const userID = JSON.parse(localStorage.getItem('user')).User.ID;
            this.ingredientService.scanQRCodeOutput(res.QRCode, this.buildingName, userID).subscribe((status: any) => {
              if (status === true) {
                this.getAllIngredientInfoOutputByBuilding();
              } else {
                this.alertify.error(status.message);
              }
            });
          } else {
            this.alertify.error('Wrong Chemical!');
          }
        }
      }));
  }

  // sau khi scan input thay doi
  async onNgModelChangeScanQRCode(args) {
    const scanner: IScanner = {
      QRCode: args,
      ingredient: null
    };
    this.subject.next(scanner);
  }

  // load danh sach IngredientInfo
  getIngredientInfo() {
    this.ingredientService.getAllIngredientInfo().subscribe((res: any) => {
      this.data = res;
      // this.ConvertClass(res);
    });
  }

  getIngredientInfoOutput() {
    this.ingredientService.getAllIngredientInfoOutput().subscribe((res: any) => {
      this.data = res;
      // this.ConvertClass(res);
    });
  }
  getAllIngredientInfoByBuilding() {
    this.ingredientService.getAllIngredientInfoByBuilding(this.buildingName).subscribe((res: any) => {
      this.data = res;
      // this.ConvertClass(res);
    });
  }

  getAllIngredientInfoOutputByBuilding() {
    this.ingredientService.getAllIngredientInfoOutputByBuilding(this.buildingName).subscribe((res: any) => {
      this.data = res;
      // this.ConvertClass(res);
    });
  }
  // tim Qrcode dang scan co ton tai khong
  findIngredientCode(code) {
    for (const item of this.ingredients) {
      if (item.materialNO === code) {
        // return true;
        this.checkCode = true;
        break;
      } else {
        this.checkCode = false;
      }
    }
  }

  // lay toan bo Ingredient
  getAllIngredient() {
    this.ingredientService.getAllIngredient().subscribe((res: any) => {
      this.ingredients = res;
    });
  }

  // dung de convert color input khi scan nhung chua can dung
  ConvertClass(res) {
    if (res.length !== 0) {
      this.test = 'form-control success-scan';
    } else {
      this.test = 'form-control error-scan';
      this.alertify.error('Wrong Chemical!');
    }
  }

  // xoa Ingredient Receiving
  delete(item) {
    this.ingredientService.deleteIngredientInfo(item.id, item.code, item.qty, item.batch).subscribe(() => {
      this.alertify.success('Delete Success!');
      this.getIngredientInfo();
      this.getIngredientInfoOutput();
    });
  }

  // luu du lieu sau khi scan Qrcode vao IngredientReport
  confirm() {
    this.alertify.confirm('Do you want confirm this', 'Do you want confirm this', () => {
      this.alertify.success('Confirm Success');
    });
  }
}
