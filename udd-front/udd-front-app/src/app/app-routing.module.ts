import { NgModule } from '@angular/core';
import { RouterModule, Routes } from '@angular/router';
import { AddApplicationComponent } from './add-application/add-application.component';
import { SearchComponent } from './search/search.component';
const routes: Routes = 
[
  { path: 'new', component: AddApplicationComponent  },
  { path: '', component: SearchComponent  },
];

@NgModule({
  imports: [RouterModule.forRoot(routes)],
exports: [RouterModule]
})
export class AppRoutingModule { }
