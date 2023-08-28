#import <Foundation/Foundation.h>
#import <UIKit/UIKit.h>
#import <WebKit/WebKit.h>

typedef void (*INT_CALLBACK)(int);

@interface IOSAppleLoginPlugin: NSObject <UIAlertViewDelegate, WKNavigationDelegate>
{
    INT_CALLBACK alertCallBack;
    NSDate *creationDate;
    INT_CALLBACK shareCallBack;
    UIPopoverController *popover;
    WKWebView *webView;
    NSURL* redirectURI;
}
@end

@implementation IOSAppleLoginPlugin

static IOSAppleLoginPlugin *_sharedInstance;

+(IOSAppleLoginPlugin*) sharedInstance
{
    static dispatch_once_t onceToken;
    dispatch_once(&onceToken, ^{
        NSLog(@"Creating IOSAppleLoginPlugin shared instance");
        _sharedInstance = [[IOSAppleLoginPlugin alloc] init];
    });
    return _sharedInstance;
}

+(NSString*)createNSString:(const char*) string {
    if (string!=nil)
        return [NSString stringWithUTF8String:string];
    else
        return @"";
}

-(id)init
{
    self = [super init];
    if (self)
        [self initHelper];
    return self;
}

-(void)initHelper
{
    NSLog(@"InitHelper called");
    creationDate = [NSDate date];
}

-(void)showWebView:(const char*)URL_in :(const char*)redirectURL_in
{
    UIView *mainView = UnityGetGLView();
    NSString *URL = [IOSAppleLoginPlugin createNSString:URL_in];
    NSString *strRedirectURL = [IOSAppleLoginPlugin createNSString:redirectURL_in];
    redirectURI = [NSURL URLWithString:strRedirectURL];
    
    WKWebViewConfiguration *configuration = [[WKWebViewConfiguration alloc] init];
    CGRect frame = mainView.frame;
    webView = [[WKWebView alloc] initWithFrame:frame configuration:configuration];
    webView.navigationDelegate = self;
    
    NSURLRequest *nsrequest=[NSURLRequest requestWithURL:[NSURL URLWithString:URL]];
    [webView loadRequest:nsrequest];
    [mainView addSubview:webView];
}

- (void)webView:(WKWebView *)webView didReceiveServerRedirectForProvisionalNavigation:(null_unspecified WKNavigation *)navigation
{
    if ([webView.URL.scheme isEqual:redirectURI.scheme] && [webView.URL.host isEqual:redirectURI.host] && [webView.URL.path isEqual:redirectURI.path]) {
        NSString *urlString = webView.URL.absoluteString;
        NSURLComponents *components = [[NSURLComponents alloc] initWithString:urlString];
        bool found = false;
        for(NSURLQueryItem *item in components.queryItems)
        {
            if([item.name isEqualToString:@"id_token"])
            {
                NSString *token = [NSString stringWithString:item.value];
                found = true;
                UnitySendMessage("IOSPortkeyAppleLoginCallback", "OnSuccess", [token UTF8String]);
            }
        }
        
        if(!found)
        {
            UnitySendMessage("IOSPortkeyAppleLoginCallback", "OnFailure", "No identity token given.");
        }
        
        [webView removeFromSuperview];
    }
    else
    {
        UnitySendMessage("IOSPortkeyAppleLoginCallback", "OnFailure", "Login flow error.");
        [webView removeFromSuperview];
    }
}

@end

extern "C"
{
    void SignInApple(const char* URL, const char* redirectURI)
    {
        NSLog(@"Called showWeb with %s",URL);
        [[IOSAppleLoginPlugin sharedInstance] showWebView:URL:redirectURI];
    }
}
