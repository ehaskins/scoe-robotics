################################################################################
# Automatically-generated file. Do not edit!
################################################################################

SHELL := cmd.exe
RM := rm -rf

USER_OBJS :=

LIBS := 
PROJ := 

O_SRCS := 
C_SRCS := 
S_SRCS := 
S_UPPER_SRCS := 
OBJ_SRCS := 
ASM_SRCS := 
PREPROCESSING_SRCS := 
OBJS := 
OBJS_AS_ARGS := 
C_DEPS := 
C_DEPS_AS_ARGS := 
EXECUTABLES := 
OUTPUT_FILE_PATH :=
OUTPUT_FILE_PATH_AS_ARGS :=
AVR_APP_PATH :=$$$AVR_APP_PATH$$$
QUOTE := "
ADDITIONAL_DEPENDENCIES:=
OUTPUT_FILE_DEP:=

# Every subdirectory with source files must be described here
SUBDIRS :=  \
../RobotModel \
../UdpComm


# Add inputs and outputs from these tool invocations to the build variables 
C_SRCS +=  \
../BalBot.cpp \
../main.cpp \
../RobotModel/Accelerometer.cpp \
../RobotModel/Gyro.cpp \
../RobotModel/PID.cpp \
../RobotModel/SimpleAngleThing.cpp \
../RobotModel/Utils.cpp \
../UdpComm/UdpComms.cpp


PREPROCESSING_SRCS += 


ASM_SRCS += 


OBJS +=  \
BalBot.o \
main.o \
RobotModel/Accelerometer.o \
RobotModel/Gyro.o \
RobotModel/PID.o \
RobotModel/SimpleAngleThing.o \
RobotModel/Utils.o \
UdpComm/UdpComms.o


OBJS_AS_ARGS +=  \
BalBot.o \
main.o \
RobotModel/Accelerometer.o \
RobotModel/Gyro.o \
RobotModel/PID.o \
RobotModel/SimpleAngleThing.o \
RobotModel/Utils.o \
UdpComm/UdpComms.o


C_DEPS +=  \
BalBot.d \
main.d \
RobotModel/Accelerometer.d \
RobotModel/Gyro.d \
RobotModel/PID.d \
RobotModel/SimpleAngleThing.d \
RobotModel/Utils.d \
UdpComm/UdpComms.d


C_DEPS_AS_ARGS +=  \
BalBot.d \
main.d \
RobotModel/Accelerometer.d \
RobotModel/Gyro.d \
RobotModel/PID.d \
RobotModel/SimpleAngleThing.d \
RobotModel/Utils.d \
UdpComm/UdpComms.d


OUTPUT_FILE_PATH +=BalBot.elf

OUTPUT_FILE_PATH_AS_ARGS +=BalBot.elf

ADDITIONAL_DEPENDENCIES:=

OUTPUT_FILE_DEP:= ./makedep.mk

# AVR32/GNU C Compiler

















./%.o: .././%.cpp
	@echo Building file: $<
	@echo Invoking: AVR8/GNU C++ Compiler : (AVR_8_bit_GNU_Toolchain_3.4.0_663) 4.6.2
	$(QUOTE)C:\Program Files (x86)\Atmel\Atmel Studio 6.0\extensions\Atmel\AVRGCC\3.4.0.65\AVRToolchain\bin\avr-g++.exe$(QUOTE) -funsigned-char -funsigned-bitfields -I"../../ArduinoCore" -I"../../ScoeComms"  -O1 -fpack-struct -fshort-enums -g2 -Wall -c -MD -MP -MF "$(@:%.o=%.d)" -MT"$(@:%.o=%.d)" -MT"$(@:%.o=%.o)"  -mmcu=atmega2560   -o"$@" "$<" 
	@echo Finished building: $<

RobotModel/%.o: ../RobotModel/%.cpp
	@echo Building file: $<
	@echo Invoking: AVR8/GNU C++ Compiler : (AVR_8_bit_GNU_Toolchain_3.4.0_663) 4.6.2
	$(QUOTE)C:\Program Files (x86)\Atmel\Atmel Studio 6.0\extensions\Atmel\AVRGCC\3.4.0.65\AVRToolchain\bin\avr-g++.exe$(QUOTE) -funsigned-char -funsigned-bitfields -I"../../ArduinoCore" -I"../../ScoeComms"  -O1 -fpack-struct -fshort-enums -g2 -Wall -c -MD -MP -MF "$(@:%.o=%.d)" -MT"$(@:%.o=%.d)" -MT"$(@:%.o=%.o)"  -mmcu=atmega2560   -o"$@" "$<" 
	@echo Finished building: $<

UdpComm/%.o: ../UdpComm/%.cpp
	@echo Building file: $<
	@echo Invoking: AVR8/GNU C++ Compiler : (AVR_8_bit_GNU_Toolchain_3.4.0_663) 4.6.2
	$(QUOTE)C:\Program Files (x86)\Atmel\Atmel Studio 6.0\extensions\Atmel\AVRGCC\3.4.0.65\AVRToolchain\bin\avr-g++.exe$(QUOTE) -funsigned-char -funsigned-bitfields -I"../../ArduinoCore" -I"../../ScoeComms"  -O1 -fpack-struct -fshort-enums -g2 -Wall -c -MD -MP -MF "$(@:%.o=%.d)" -MT"$(@:%.o=%.d)" -MT"$(@:%.o=%.o)"  -mmcu=atmega2560   -o"$@" "$<" 
	@echo Finished building: $<



# AVR32/GNU Preprocessing Assembler



# AVR32/GNU Assembler




ifneq ($(MAKECMDGOALS),clean)
ifneq ($(strip $(C_DEPS)),)
-include $(C_DEPS)
endif
endif

# Add inputs and outputs from these tool invocations to the build variables 

# All Target
all: $(OUTPUT_FILE_PATH) $(ADDITIONAL_DEPENDENCIES)

$(OUTPUT_FILE_PATH): $(OBJS) $(USER_OBJS) $(OUTPUT_FILE_DEP)
	@echo Building target: $@
	@echo Invoking: AVR8/GNU Linker : (AVR_8_bit_GNU_Toolchain_3.4.0_663) 4.6.2
	$(QUOTE)C:\Program Files (x86)\Atmel\Atmel Studio 6.0\extensions\Atmel\AVRGCC\3.4.0.65\AVRToolchain\bin\avr-g++.exe$(QUOTE) -o$(OUTPUT_FILE_PATH_AS_ARGS) $(OBJS_AS_ARGS) $(USER_OBJS) $(LIBS) -Wl,-Map="BalBot.map" -Wl,--start-group -Wl,-lm -Wl,-lArduino -Wl,-lScoeComms  -Wl,--end-group -Wl,-L"../../ArduinoCore/Debug - 2560" -Wl,-L"../../ScoeComms/Debug - 2560"   -mmcu=atmega2560  
	@echo Finished building target: $@
	"C:\Program Files (x86)\Atmel\Atmel Studio 6.0\extensions\Atmel\AVRGCC\3.4.0.65\AVRToolchain\bin\avr-objcopy.exe" -O ihex -R .eeprom -R .fuse -R .lock -R .signature  "BalBot.elf" "BalBot.hex"
	"C:\Program Files (x86)\Atmel\Atmel Studio 6.0\extensions\Atmel\AVRGCC\3.4.0.65\AVRToolchain\bin\avr-objcopy.exe" -j .eeprom  --set-section-flags=.eeprom=alloc,load --change-section-lma .eeprom=0  --no-change-warnings -O ihex "BalBot.elf" "BalBot.eep" || exit 0
	"C:\Program Files (x86)\Atmel\Atmel Studio 6.0\extensions\Atmel\AVRGCC\3.4.0.65\AVRToolchain\bin\avr-objdump.exe" -h -S "BalBot.elf" > "BalBot.lss"
	"C:\Program Files (x86)\Atmel\Atmel Studio 6.0\extensions\Atmel\AVRGCC\3.4.0.65\AVRToolchain\bin\avr-size.exe" "BalBot.elf"
	





# Other Targets
clean:
	-$(RM) $(OBJS_AS_ARGS)$(C_DEPS_AS_ARGS) $(EXECUTABLES) 
	rm -rf "BalBot.elf" "BalBot.a" "BalBot.hex" "BalBot.lss" "BalBot.eep" "BalBot.map"
	