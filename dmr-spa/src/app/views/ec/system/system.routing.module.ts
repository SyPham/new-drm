import { FunctionComponent } from './function/function.component';
import { NgModule } from "@angular/core";
import { RouterModule, Routes } from "@angular/router";
import { ModuleComponent } from "./module/module.component";
import { ActionComponent } from './action/action.component';
import { RoleComponent } from './role/role.component';

const routes: Routes = [
  {
    path: '',
    children: [
      {
        path: 'module',
        component: ModuleComponent,
        data: {
          title: 'Module'
        }
      },
      {
        path: 'function',
        component: FunctionComponent,
        data: {
          title: 'Function'
        }
      },
      {
        path: 'action',
        component: ActionComponent,
        data: {
          title: 'Action'
        }
      },
      {
        path: 'role',
        component: RoleComponent,
        data: {
          title: 'Role'
        }
      }
    ]
  }
];
@NgModule({
  imports: [RouterModule.forChild(routes)],
  exports: [RouterModule]
})
export class ECRoutingModule { }
