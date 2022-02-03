import { Component, OnInit } from '@angular/core';
import { FormControl, FormGroup, Validators } from '@angular/forms';
import { CvService } from './../services/cv/cv.service';
import { SearchResult } from './../models/content-search-result.model';
import { CombinedSearchQuery } from './../models/combined-search.model';
import { ValidationService } from './../services/cv/validation.service';
import { CityService } from './../services/cv/city.service';
import { City } from '../models/city.model';

@Component({
  selector: 'app-search',
  templateUrl: './search.component.html',
  styleUrls: ['./search.component.css']
})
export class SearchComponent implements OnInit {

  searchForm = new FormGroup(
    {
      nameControl:new FormControl(''),
      phraseControl:new FormControl(''),
      lastNameControl:new FormControl(''),
      edLevelControl:new FormControl('1'),
      cityControl:new FormControl(''),
      radiusControl:new FormControl(''),
      coverLetterControl:new FormControl(''),
    }
  );

  combinedSearchForm = new FormGroup(
    {
      nameControl:new FormControl('', Validators.required),
      lastNameControl:new FormControl('', Validators.required),
      edLevelControl:new FormControl('1', Validators.required),
      coverLetterControl:new FormControl('', Validators.required),
      op1Control:new FormControl('AND', Validators.required),
      op2Control:new FormControl('AND', Validators.required),
      op3Control:new FormControl('AND', Validators.required),
    }
  );
  applications:SearchResult[] = [];
  isLoading:boolean = false;
  cities:City[] = [];

  constructor(private cvService:CvService, private cityService:CityService, private validation:ValidationService) { }

  ngOnInit(): void {
    this.loadCities();

  }

  loadCities(){
    this.cityService.getCities()
    .subscribe(
      data =>{
        this.cities = data
      }
    )
  }

  searchBoolean(){
    if(!this.combinedSearchForm.valid)
    {
      this.validation.validateAllFields(this.combinedSearchForm);
      return; 
    }
    this.isLoading = true;
    let name = this.combinedSearchForm.controls['nameControl'].value
    let lastName = this.combinedSearchForm.controls['lastNameControl'].value
    let ed = this.combinedSearchForm.controls['edLevelControl'].value
    let cv = this.combinedSearchForm.controls['coverLetterControl'].value
    let op1 = this.combinedSearchForm.controls['op1Control'].value
    let op2 = this.combinedSearchForm.controls['op2Control'].value
    let op3 = this.combinedSearchForm.controls['op3Control'].value

    let query = new CombinedSearchQuery()
    query.applicantName = name;
    query.applicantLastname = lastName;
    query.applicantEducationlevel = ed;
    query.cvContent = cv;
    query.operator1 = op1;
    query.operator2 = op2;
    query.operator3 = op3;
      this.cvService.getBoolean(query)
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

  searchByPhrase()
  {
    this.isLoading = true;
    let phrase = this.searchForm.controls['phraseControl'].value
    this.cvService.getByPhrase(phrase)
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
