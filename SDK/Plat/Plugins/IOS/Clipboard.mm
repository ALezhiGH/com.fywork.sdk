#import"Clipboard.h"
#import <AppTrackingTransparency/AppTrackingTransparency.h>
#import <AdSupport/AdSupport.h>
@implementation Clipboard
//将文本复制到IOS剪贴板
- (void)objc_copyTextToClipboard : (NSString*)text
{
     UIPasteboard *pasteboard = [UIPasteboard generalPasteboard];
     pasteboard.string = text;

}
-(void)RequestIDFA
{
    if (@available(iOS 14, *)) {
            // iOS14及以上版本需要先请求权限
            [ATTrackingManager requestTrackingAuthorizationWithCompletionHandler:^(ATTrackingManagerAuthorizationStatus status) {
          
            }];
        }
}
@end

extern "C" {
     static Clipboard *iosClipboard;
   
     void _copyTextToClipboard(const char *textList)
    {
        NSString *text = [NSString stringWithUTF8String: textList] ;
       
        if(iosClipboard == NULL)
        {
            iosClipboard = [[Clipboard alloc] init];
        }
       
        [iosClipboard objc_copyTextToClipboard: text];
    }
	void GetIDFA()
	{
		if(iosClipboard == NULL)
		{
			iosClipboard = [[Clipboard alloc] init];
		}
		[iosClipboard RequestIDFA];
	}
	BOOL iOSJoinQQGroup(const char* rawKey,const char* rawUid){
        NSString * key = [NSString stringWithUTF8String:rawKey];
        NSString * uid = [NSString stringWithUTF8String:rawUid];
        NSString *urlStr = [NSString stringWithFormat:@"mqqapi://card/show_pslcard?src_type=internal&version=1&uin=%@&key=%@&card_type=group&source=external&jump_from=webapi", uid,key];
        NSURL *url = [NSURL URLWithString:urlStr];
        if([[UIApplication sharedApplication] canOpenURL:url]){
        [[UIApplication sharedApplication] openURL:url];
            return YES;
        }
    else return NO;
    }


}