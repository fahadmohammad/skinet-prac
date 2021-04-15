// This file can be replaced during build by using the `fileReplacements` array.
// `ng build --prod` replaces `environment.ts` with `environment.prod.ts`.
// The list of file replacements can be found in `angular.json`.

export const environment = {
  production: false,
  apiUrl: 'https://localhost:5001/api/',
  googleClientId:'168152023048-8vjcmag46g0tedhm26nl5nppvtdrg7lh.apps.googleusercontent.com',
  emailConfirmationCallback:'http://localhost:4200/account/emailconfirmation',
  passwordResetCallback: "http://localhost:4200/account/resetpassword"
};

/*
 * For easier debugging in development mode, you can import the following file
 * to ignore zone related error stack frames such as `zone.run`, `zoneDelegate.invokeTask`.
 *
 * This import should be commented out in production mode because it will have a negative impact
 * on performance if an error is thrown.
 */
// import 'zone.js/dist/zone-error';  // Included with Angular CLI.
