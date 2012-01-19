/*
 * AnalogIODefinition.cpp
 *
 *  Created on: Jan 18, 2012
 *      Author: EHaskins
 */

#include "AnalogIODefinition.h"
#include <WProgram.h>
AnalogIODefinition::AnalogIODefinition() {
	// TODO Auto-generated constructor stub

}

void AnalogIODefinition::update(){
	value = (unsigned int)analogRead(pin);
}
