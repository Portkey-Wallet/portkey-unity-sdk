#import <Foundation/Foundation.h>
#import <UIKit/UIKit.h>
#import <WebKit/WebKit.h>

typedef void (*INT_CALLBACK)(int);

@interface IOSAppleLoginPlugin: NSObject <WKUIDelegate, WKNavigationDelegate>
{
    INT_CALLBACK alertCallBack;
    NSDate *creationDate;
    INT_CALLBACK shareCallBack;
    UIPopoverController *popover;
    WKWebView *webView;
    NSURL* _redirectURI;
    NSURL* _URL;
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
    _redirectURI = [NSURL URLWithString:strRedirectURL];
    NSString *strURL = [IOSAppleLoginPlugin createNSString:URL_in];
    _URL = [NSURL URLWithString:strURL];
    
    WKWebViewConfiguration *configuration = [[WKWebViewConfiguration alloc] init];
    CGRect frame = mainView.frame;
    webView = [[WKWebView alloc] initWithFrame:frame configuration:configuration];
    webView.navigationDelegate = self;
    webView.UIDelegate = self;
    webView.opaque = NO;
    //webView.backgroundColor = [UIColor clearColor];
    
    [mainView addSubview:webView];
    NSURLRequest *nsrequest=[NSURLRequest requestWithURL:[NSURL URLWithString:URL]];
    [webView loadRequest:nsrequest];
}

- (void)webView:(WKWebView *)webView didStartProvisionalNavigation:(null_unspecified WKNavigation *)navigation
{
    // if we are redirecting, we just hide the webview
    if (![webView.URL.scheme isEqual:_URL.scheme] || ![webView.URL.host isEqual:_URL.host]) {
        webView.hidden = YES;
    }
}

- (void)webView:(WKWebView *)webView didFinishNavigation:(null_unspecified WKNavigation *)navigation
{
    webView.opaque = YES;
}

- (void)webView:(WKWebView *)webView didFailProvisionalNavigation:(null_unspecified WKNavigation *)navigation withError:(nonnull NSError *)error
{
    UnitySendMessage("IOSPortkeyAppleLoginCallback", "OnFailure", "Login Cancelled.");
    [webView removeFromSuperview];
}

- (void)webView:(WKWebView *)webView didFailNavigation:(null_unspecified WKNavigation *)navigation withError:(nonnull NSError *)error
{
    UnitySendMessage("IOSPortkeyAppleLoginCallback", "OnFailure", "Login Cancelled.");
    [webView removeFromSuperview];
}

- (void)webViewWebContentProcessDidTerminate:(WKWebView *)webView
{
    UnitySendMessage("IOSPortkeyAppleLoginCallback", "OnFailure", "Login Cancelled.");
    [webView removeFromSuperview];
}

- (void)webViewDidClose:(WKWebView *)webView
{
    UnitySendMessage("IOSPortkeyAppleLoginCallback", "OnFailure", "Login Cancelled.");
    [webView removeFromSuperview];
}

- (void)webView:(WKWebView *)webView didReceiveServerRedirectForProvisionalNavigation:(null_unspecified WKNavigation *)navigation
{
    if ([webView.URL.scheme isEqual:_redirectURI.scheme] && [webView.URL.host isEqual:_redirectURI.host] && [webView.URL.path isEqual:_redirectURI.path]) {
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
