import { Component, OnInit } from '@angular/core';
import { FormControl, FormGroup } from '@angular/forms';
import { CvService } from './../services/cv/cv.service';
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
      edLevelControl:new FormControl('1'),
      cityControl:new FormControl(''),
      radiusControl:new FormControl(''),
      coverLetterControl:new FormControl(''),
    }
  );
  applications:SearchResult[] = [];
  isLoading:boolean = false;

  constructor(private cvService:CvService) { }

  ngOnInit(): void {
  }

  search(){

  }

  searchByEducation()
  {
    this.isLoading = true;
      this.cvService.getByEducationLevel(this.searchForm.controls['edLevelControl'].value)
      .subscribe(
        data =>{
            this.isLoading = false;
            this.applications = data;
        },

        error =>{
        //handle error here
        this.isLoading = false;
        }
      )
  }

  searchByName()
  {
    this.isLoading = true;
    let name = this.searchForm.controls['nameControl'].value
    let lastName = this.searchForm.controls['lastNameControl'].value
    this.cvService.getByNameLastname(name, lastName)
      .subscribe(
        data =>{
            this.isLoading = false;
            this.applications = data;
        },

        error =>{
          this.isLoading = false;
        //handle error here
        }
      )
  }

  searchByGeo()
  {
    this.isLoading = true;
    let radius =  this.searchForm.controls['radiusControl'].value
    let city =  this.searchForm.controls['cityControl'].value
    this.cvService.getByGeoLocation(city, radius)
      .subscribe(
        data =>{
            this.isLoading = false;
            this.applications = data;
        },

        error =>{
          this.isLoading = false;
        //handle error here
        }
      )
  }

  searchByContent()
  {
    this.isLoading = true;
    let content =  this.searchForm.controls['coverLetterControl'].value
    this.cvService.getByContent(content)
      .subscribe(
        data =>{
          this.isLoading = false;
          this.applications = data;
        },

        error =>{
          this.isLoading = false;
        //handle error here
        }
      )
  }

}
