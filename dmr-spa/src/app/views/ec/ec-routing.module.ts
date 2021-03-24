import { NgModule } from '@angular/core';
import { Routes, RouterModule } from '@angular/router';
const routes: Routes = [
  {
    path: '',
    data: {
      title: 'ec',
      breadcrumb: 'Home'
    },
    children: [
      {
        path: 'setting',
        loadChildren: () =>
          import('./setting/setting.module').then(m => m.SettingModule)
      },
      {
        path: 'establish',
        loadChildren: () =>
          import('./establish/establish.module').then(m => m.EstablishModule)
      },
      {
        path: 'troubleshooting',
        loadChildren: () =>
          import('./troubleshooting/troubleshooting.module').then(m => m.TroubleshootingModule)
      },
      // execution
      {
        path: 'execution',
        loadChildren: () =>
          import('./execution/execution.module').then(m => m.ExecutionModule)
      },
      // end execution

       // report
      {
        path: 'report',
        loadChildren: () =>
          import('./report/report.module').then(m => m.ReportModule)
      },
      // end report
      {
        path: 'system',
        loadChildren: () =>
          import('./system/system.module').then(m => m.SystemModule)
      },
    ]
  }
];
@NgModule({
  imports: [RouterModule.forChild(routes)],
  exports: [RouterModule]
})
export class ECRoutingModule { }
