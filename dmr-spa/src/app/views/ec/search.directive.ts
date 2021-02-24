import { Directive, AfterViewInit, ElementRef, Input, OnChanges, HostListener } from '@angular/core';

@Directive({
  // tslint:disable-next-line:directive-selector
  selector: '[autofocusSearch]'
})
export class SearchDirective implements AfterViewInit {
  @HostListener('ngModelChange', ['$event']) onChange(value) {
    const input = value.split('-') || [];
    if (input[2]?.length === 8) {
      setTimeout(() => {
        this.host.nativeElement.select();
      }, 2000);
    }
  }
  @HostListener('focus') onBlur() {
    setTimeout( () => {
      this.host.nativeElement.select();
    }, 2000);
  }

  constructor(private host: ElementRef) { }
  ngAfterViewInit() {
      setTimeout(() => {
        this.host.nativeElement.focus();
        this.host.nativeElement.select();
      }, 2000);
    }
  @HostListener('document:keydown.enter', ['$event'])
  onKeydownHandler(event: KeyboardEvent) {
    event.preventDefault();
    this.host.nativeElement.value = this.host.nativeElement.value + '    ';
  }
  @HostListener('document:keydown.tab', ['$event'])
  onKeydownTabHandler(event: KeyboardEvent) {
    event.preventDefault();
    this.host.nativeElement.value = this.host.nativeElement.value + '    ';
  }
}
