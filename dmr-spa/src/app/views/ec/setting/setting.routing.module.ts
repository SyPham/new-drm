import { NgModule } from '@angular/core';
import { RouterModule, Routes } from '@angular/router';
import {
  AccountComponent, BuildingComponent, BuildingLunchTimeComponent,
  BuildingSettingComponent, CostingComponent, DecentralizationComponent,
  GlueTypeComponent,
  IngredientComponent, KindComponent, LunchTimeComponent, MailingComponent,
  MaterialComponent, PartComponent, PrintQRCodeComponent, ScalingSettingComponent,
  SubpackageCapacityComponent, SuppilerComponent
} from '.';
const routes: Routes = [
  {
    path: '',
    children: [
      {
        path: 'account',
        component: AccountComponent,
        data: {
          title: 'Account',
          breadcrumb: 'Account'
        }
      },
      {
        path: 'building',
        component: BuildingComponent,
        data: {
          title: 'Building',
          breadcrumb: 'Building'
        }
      },
      {
        path: 'supplier',
        component: SuppilerComponent,
        data: {
          title: 'Supplier',
          breadcrumb: 'Supplier'
        }
      },
      {
        path: 'chemical',
        data: {
          title: 'Chemical',
          breadcrumb: 'Chemical'
        },
        children: [
          {
            path: '',
            component: IngredientComponent,
          },
          {
            path: 'print-qrcode/:id/:code/:name',
            component: PrintQRCodeComponent,
            data: {
              title: 'Print QRCode',
              breadcrumb: 'Print QRCode'
            }
          }
        ]
      },
      {
        path: 'kind',
        component: KindComponent,
        data: {
          title: 'Kind',
          breadcrumb: 'Kind'
        }
      },
      {
        path: 'part',
        component: PartComponent,
        data: {
          title: 'Part',
          breadcrumb: 'Part'
        }
      },
      {
        path: 'material',
        component: MaterialComponent,
        data: {
          title: 'Material',
          breadcrumb: 'Material'
        }
      },
      {
        path: 'glue-type',
        component: GlueTypeComponent,
        data: {
          title: 'Glue Type',
          breadcrumb: 'Glue Type'
        }
      },
      {
        path: 'building-setting',
        component: BuildingSettingComponent,
        data: {
          title: 'Building Setting',
          breadcrumb: 'Building Setting'
        }
      },
      {
        path: 'building-lunch-time',
        component: BuildingLunchTimeComponent,
        data: {
          title: 'Building Lunch Time',
          breadcrumb: 'Building Lunch Time'
        }
      },
      {
        path: 'decentralization',
        component: DecentralizationComponent,
        data: {
          title: 'Decentralization',
          breadcrumb: 'Decentralization'
        }
      },
      {
        path: 'scaling-setting',
        component: ScalingSettingComponent,
        data: {
          title: 'Scaling Setting',
          breadcrumb: 'Scaling Setting'
        }
      },
      {
        path: 'mailing',
        component: MailingComponent,
        data: {
          title: 'Mailing',
          breadcrumb: 'Mailing'
        }
      },
      {
        path: 'lunch-time',
        component: LunchTimeComponent,
        data: {
          title: 'Lunch Time',
          breadcrumb: 'Lunch Time'
        }
      },
      {
        path: 'subpackage-capacity',
        component: SubpackageCapacityComponent,
        data: {
          title: 'Subpackage Capacity',
          breadcrumb: 'Subpackage Capacity'
        }
      },
      {
        path: 'costing',
        component: CostingComponent,
        data: {
          title: 'costing',
          breadcrumb: 'Costing'
        }
      },
    ]
  }
];
@NgModule({
  imports: [RouterModule.forChild(routes)],
  exports: [RouterModule]
})
export class ECRoutingModule { }
