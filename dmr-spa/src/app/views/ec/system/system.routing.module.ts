import { VersionAddComponent } from './version/version-add/version-add.component';
import { ActionFunctionComponent } from './action-function/action-function.component';
import { FunctionComponent } from './function/function.component';
import { NgModule } from "@angular/core";
import { RouterModule, Routes } from "@angular/router";
import { ModuleComponent } from "./module/module.component";
import { ActionComponent } from './action/action.component';
import { RoleComponent } from './role/role.component';
import { VersionComponent } from './version/version.component';

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
      },
      {
        path: 'action-in-function',
        component: ActionFunctionComponent,
        data: {
          title: 'Action In Function'
        }
      },
      {
        path: 'version',
        data: {
          title: 'verison'
        },
        children: [
          {
            path: '',
            component: VersionComponent
          },
         {
            path: 'add',
            component: VersionAddComponent,
            data: {
              title: 'Add'
            }
         },
          {
            path: 'edit/:id',
            component: VersionAddComponent,
            data: {
              title: 'Edit'
            }
          }
        ]
      }
    ]
  }
];
@NgModule({
  imports: [RouterModule.forChild(routes)],
  exports: [RouterModule]
})
export class ECRoutingModule { }
