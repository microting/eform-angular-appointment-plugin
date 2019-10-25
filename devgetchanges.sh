#!/bin/bash

cd ~

if [ -d "Documents/workspace/microting/eform-angular-appointment-plugin/eform-client/src/app/plugins/modules/appointment-pn" ]; then
	rm -fR Documents/workspace/microting/eform-angular-appointment-plugin/eform-client/src/app/plugins/modules/appointment-pn
fi

cp -av Documents/workspace/microting/eform-angular-frontend/eform-client/src/app/plugins/modules/appointment-pn Documents/workspace/microting/eform-angular-appointment-plugin/eform-client/src/app/plugins/modules/appointment-pn

if [ -d "Documents/workspace/microting/eform-angular-appointment-plugin/eFormAPI/Plugins/Appointment.Pn" ]; then
	rm -fR Documents/workspace/microting/eform-angular-appointment-plugin/eFormAPI/Plugins/Appointment.Pn
fi

cp -av Documents/workspace/microting/eform-angular-frontend/eFormAPI/Plugins/Appointment.Pn Documents/workspace/microting/eform-angular-appointment-plugin/eFormAPI/Plugins/Appointment.Pn
