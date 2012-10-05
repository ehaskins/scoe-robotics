/*
 * Utils.h
 *
 *  Created on: Apr 4, 2011
 *      Author: EHaskins
 */

#ifndef UTILS_H_
#define UTILS_H_

int deadband(int value, int deadband);
int limit(int value, int min, int max);
double removeDeadband(double value, double deadband, double range);
double limit(double value, double min, double max);
#endif /* UTILS_H_ */
