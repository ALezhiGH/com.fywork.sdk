//
//  GetID.m
//  Unity-iPhone
//
//  Created by Weily on 15/5/4.
//
//

//#import <AdSupport/ASIdentifierManager.h>

#import <Foundation/Foundation.h>
#include <sys/sysctl.h>
#import <AVFoundation/AVFoundation.h>
extern "C"{
   double GetStartUpTime()
{
    struct timeval boottime;
    int mib[2] = {CTL_KERN, KERN_BOOTTIME};
    size_t size = sizeof(boottime);
    time_t now;
    time_t uptime = -1;
    (void)time(&now);
    if (sysctl(mib, 2, &boottime, &size, NULL, 0) != -1 && boottime.tv_sec != 0)
    {
        uptime = now - boottime.tv_sec;
    }
    
    return uptime;
}
}
