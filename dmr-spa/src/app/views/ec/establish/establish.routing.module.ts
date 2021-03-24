import { NgModule } from "@angular/core";
import { RouterModule, Routes } from "@angular/router";
import { BPFCScheduleComponent, BpfcDetailComponent,
  BpfcStatusComponent, Bpfc1Component } from ".";
const routes: Routes = [
  {
    path: '',
    children: [
      {
        path: 'bpfc-schedule',
        data: {
          title: 'bpfc-schedule',
          breadcrumb: 'BPFC Schedule'
        },
        children: [
          {
            path: '',
            component: BPFCScheduleComponent
          },
          {
            path: 'detail',
            data: {
              title: 'Detail',
              breadcrumb: 'Detail'
            },
            children: [
              {
                path: '',
                component: BpfcDetailComponent,
              },
              {
                path: ':id',
                component: BpfcDetailComponent,
              }
            ]
          },

        ]
      },
      {
        path: 'bpfc-status',
        component: BpfcStatusComponent,
        data: {
          title: 'BPFC Status',
          breadcrumb: 'BPFC Status'
        }
      },
      {
        path: 'bpfc-1',
        component: Bpfc1Component,
        data: {
          title: 'BPFC ',
          breadcrumb: 'BPFC '
        }
      }
    ]
  }
];
@NgModule({
  imports: [RouterModule.forChild(routes)],
  exports: [RouterModule]
})
export class EstablishRoutingModule { }
