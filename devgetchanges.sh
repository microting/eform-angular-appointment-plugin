#!/bin/bash

cd ~

rm -fR Documents/workspace/microting/eform-angular-appointment-plugin/eform-client/src/app/plugins/modules/appointment-pn

cp -a Documents/workspace/microting/eform-angular-frontend/eform-client/src/app/plugins/modules/appointment-pn Documents/workspace/microting/eform-angular-appointment-plugin/eform-client/src/app/plugins/modules/appointment-pn

rm -fR Documents/workspace/microting/eform-angular-appointment-plugin/eFormAPI/Plugins/Appointment.Pn

cp -a Documents/workspace/microting/eform-angular-frontend/eFormAPI/Plugins/Appointment.Pn Documents/workspace/microting/eform-angular-appointment-plugin/eFormAPI/Plugins/Appointment.Pn

# Test files rm
rm -fR Documents/workspace/microting/eform-angular-appointment-plugin/eform-client/e2e/Tests/appointment-settings/
rm -fR Documents/workspace/microting/eform-angular-appointment-plugin/eform-client/e2e/Tests/appointment-general/
rm -fR Documents/workspace/microting/eform-angular-appointment-plugin/eform-client/e2e/Page\ objects/Appointment/
rm -fR Documents/workspace/microting/eform-angular-appointment-plugin/eform-client/wdio-headless-plugin-step2.conf.js

# Test files cp
cp -a Documents/workspace/microting/eform-angular-frontend/eform-client/e2e/Tests/appointment-settings Documents/workspace/microting/eform-angular-appointment-plugin/eform-client/e2e/Tests/appointment-settings
cp -a Documents/workspace/microting/eform-angular-frontend/eform-client/e2e/Tests/appointment-general Documents/workspace/microting/eform-angular-appointment-plugin/eform-client/e2e/Tests/appointment-general
cp -a Documents/workspace/microting/eform-angular-frontend/eform-client/e2e/Page\ objects/Appointment Documents/workspace/microting/eform-angular-appointment-plugin/eform-client/e2e/Page\ objects/Appointment
cp -a Documents/workspace/microting/eform-angular-frontend/eform-client/wdio-plugin-step2.conf.js Documents/workspace/microting/eform-angular-appointment-plugin/eform-client/wdio-headless-plugin-step2.conf.js

