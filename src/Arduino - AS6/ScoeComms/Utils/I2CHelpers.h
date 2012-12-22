/*
* I2CHelpers.h
*
* Created: 12/21/2012 7:42:45 PM
*  Author: EHaskins
*/


#ifndef I2CHELPERS_H_
#define I2CHELPERS_H_

void readI2CBytes(uint8_t device, uint8_t address, uint8_t *buffer, int offset, uint8_t count);
uint8_t writeI2CByte(uint8_t device, uint8_t address, uint8_t val);

#endif /* I2CHELPERS_H_ */