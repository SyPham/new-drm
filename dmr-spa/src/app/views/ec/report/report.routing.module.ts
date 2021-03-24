import { NgModule } from "@angular/core";
import { RouterModule, Routes } from "@angular/router";
import { DeliveredHistoryComponent } from "../execution/delivered-history/delivered-history.component";
import { InventoryComponent } from "../inventory/inventory.component";
import { Consumption1Component } from "./consumption-1/consumption-1.component";
import { Consumption2Component } from "./consumption-2/consumption-2.component";
import { ConsumptionComponent } from "./consumption/consumption.component";

const routes: Routes = [
  {
    path: '',
    data: {
      title: 'Report',
      breadcrumb: 'Report'
    },
    children: [
      {
        path: 'consumption',
        component: ConsumptionComponent,
        data: {
          title: 'Cost',
          breadcrumb: 'Cost'
        }
      },
      {
        path: 'consumption-1',
        component: Consumption1Component,
        data: {
          title: 'Consumption 1',
          breadcrumb: 'Consumption 1'
        }
      },
      {
        path: 'consumption-2',
        component: Consumption2Component,
        data: {
          title: 'Consumption 2',
          breadcrumb: 'Consumption 2'
        }
      },
      {
        path: 'delivered-history',
        component: DeliveredHistoryComponent,
        data: {
          title: 'Delivered History',
          breadcrumb: 'Delivered History'
        }
      },
      {
        path: 'inventory',
        component: InventoryComponent,
        data: {
          title: 'Inventory',
          breadcrumb: 'Inventory'
        },
      }
    ]
  },
];
@NgModule({
  imports: [RouterModule.forChild(routes)],
  exports: [RouterModule]
})
export class ReportRoutingModule { }
