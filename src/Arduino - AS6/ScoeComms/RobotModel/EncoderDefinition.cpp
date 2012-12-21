/*
 * EncoderDefinition.cpp
 *
 *  Created on: Feb 19, 2012
 *      Author: EHaskins
 */

#include "EncoderDefinition.h"

EncoderDefinition::EncoderDefinition(){

}
void EncoderDefinition::init(unsigned char newpinA, unsigned char newpinB){
	encoder = new Encoder(newpinA, newpinB);
	pinA = newpinA;
	pinB = newpinB;
	isConfigured = true;
}
long EncoderDefinition::read(){
	return encoder->read();
}
