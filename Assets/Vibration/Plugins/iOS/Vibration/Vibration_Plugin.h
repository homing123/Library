//
//  Vibration_Plugin.cpp
//  Unity-iPhone
//
//  Created by Sususoft on 2023/04/11.
//

#import <Foundation/Foundation.h>
#import <UIKit/UIKit.h>
#import <AudioToolbox/AudioToolbox.h>

@interface Vibration_Plugin : NSObject

#ifdef __cplusplus
extern "C" {
#endif

    void vibrateWithIdx(int idx);

#ifdef __cplusplus
}
#endif



@end
