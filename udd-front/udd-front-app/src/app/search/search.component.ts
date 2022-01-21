import { Component, OnInit } from '@angular/core';
import { FormControl, FormGroup } from '@angular/forms';
import { CvService } from './../services/cv/cv.service';
import { JobApplication } from './../models/job-application.model';
import { SearchResult } from './../models/content-search-result.model';

@Component({
  selector: 'app-search',
  templateUrl: './search.component.html',
  styleUrls: ['./search.component.css']
})
export class SearchComponent implements OnInit {

  searchForm = new FormGroup(
    {
      nameControl:new FormControl(''),
      lastNameControl:new FormControl(''),
      edLevelControl:new FormControl('all'),
      cityControl:new FormControl(''),
      radiusControl:new FormControl(''),
      coverLetterControl:new FormControl(''),
    }
  );
  applications:SearchResult[] = [];

  constructor(private cvService:CvService) { }

  ngOnInit(): void {
  }

  search(){

  }

  searchByEducation()
  {
      this.cvService.getByEducationLevel(this.searchForm.controls['edLevelControl'].value)
      .subscribe(
        data =>{
            this.applications = data;
        },

        error =>{
        //handle error here
        }
      )
  }

  searchByName()
  {
    let name = this.searchForm.controls['nameControl'].value
    let lastName = this.searchForm.controls['lastNameControl'].value
    this.cvService.getByNameLastname(name, lastName)
      .subscribe(
        data =>{
            this.applications = data;
        },

        error =>{
        //handle error here
        }
      )
  }

  searchByGeo()
  {
    let radius =  this.searchForm.controls['radiusControl'].value
    let city =  this.searchForm.controls['cityControl'].value
    this.cvService.getByGeoLocation(city, radius)
      .subscribe(
        data =>{
            this.applications = data;
        },

        error =>{
        //handle error here
        }
      )
  }

  searchByContent()
  {
    let content =  this.searchForm.controls['coverLetterControl'].value
    this.cvService.getByContent(content)
      .subscribe(
        data =>{
            this.applications = data;
        },

        error =>{
        //handle error here
        }
      )
  }

}
