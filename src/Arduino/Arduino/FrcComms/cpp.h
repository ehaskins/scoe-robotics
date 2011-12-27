/*
 * cpp.h
 *
 *  Created on: Apr 4, 2011
 *      Author: EHaskins
 */

#ifndef CPP_H_
#define CPP_H_


#include <stdlib.h>

void * operator new(size_t size);
void operator delete(void * ptr);


#endif /* CPP_H_ */
