## A Simple preloader for  winforms (With progress bar)

```cs
using Kimtoo.Preloader;
```


```cs

 //Applies to user controls and Forms
 this.ShowLoader();                   //Shows Loading...
 this.ShowLoader("Please Wait...."); //Shows loader with custom text...
 this.ShowLoader("Loading...",50);    //Shows loader with progress bar. (%)
 this.HideLoader();                   //Hide any instance of the loader 

```