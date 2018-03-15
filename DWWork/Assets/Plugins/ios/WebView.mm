/*
 * Copyright (C) 2011 Keijiro Takahashi
 * Copyright (C) 2012 GREE, Inc.
 *
 * This software is provided 'as-is', without any express or implied
 * warranty.  In no event will the authors be held liable for any damages
 * arising from the use of this software.
 *
 * Permission is granted to anyone to use this software for any purpose,
 * including commercial applications, and to alter it and redistribute it
 * freely, subject to the following restrictions:
 *
 * 1. The origin of this software must not be misrepresented; you must not
 *    claim that you wrote the original software. If you use this software
 *    in a product, an acknowledgment in the product documentation would be
 *    appreciated but is not required.
 * 2. Altered source versions must be plainly marked as such, and must not be
 *    misrepresented as being the original software.
 * 3. This notice may not be removed or altered from any source distribution.
 */

#import <UIKit/UIKit.h>

extern UIViewController *UnityGetGLViewController();

@interface WebViewPlugin : NSObject<UIWebViewDelegate>
{
    UIWebView *webView;
    NSString *gameObjectName;
}
@end

@implementation WebViewPlugin

- (id)initWithGameObjectName:(const char *)gameObjectName_
{
    self = [super init];
    
    UIView *view = UnityGetGLViewController().view;
    if ([[UIScreen mainScreen] bounds].size.width > [[UIScreen mainScreen] bounds].size.height)
    {
        webView = [[UIWebView alloc] initWithFrame:CGRectMake(0, 0, [[UIScreen mainScreen] bounds].size.width, [[UIScreen mainScreen] bounds].size.height)];
    }
    else
    {
        webView = [[UIWebView alloc] initWithFrame:CGRectMake(0, 0, [[UIScreen mainScreen] bounds].size.height, [[UIScreen mainScreen] bounds].size.width)];
    }
    webView.delegate = self;
    [webView setHidden:YES];
    [webView setUserInteractionEnabled:NO];
    [view addSubview:webView];
    gameObjectName = [NSString stringWithUTF8String:gameObjectName_] ;
    
    return self;
}

- (void)dealloc
{
    webView.delegate = nil;
    
    [webView removeFromSuperview];
    //[webView release];
    //[gameObjectName release];
    //[super dealloc];
}

- (BOOL)webView:(UIWebView *)webView shouldStartLoadWithRequest:(NSURLRequest *)request navigationType:(UIWebViewNavigationType)navigationType
{
    NSLog(@"Ezfun:webview; shouldStartLoadWithRequest in");
    /*
     NSString *url = [[request URL] absoluteString];
     UnitySendMessage([gameObjectName UTF8String], "onLoadStart", [url UTF8String] );
     */
    return YES;
    NSLog(@"Ezfun:webview; shouldStartLoadWithRequest out");
}

- (void)webViewDidStartLoad: (UIWebView*)webView
{
    NSLog(@"Ezfun:webview; webViewDidStartLoad in");
    NSString* url = [[[webView request] URL] absoluteString];
    UnitySendMessage( [gameObjectName UTF8String], "onLoadStart", [url UTF8String] );
    NSLog(@"Ezfun:webview; webViewDidStartLoad out");
}

- (void)webViewDidFinishLoad: (UIWebView*)webView
{
    NSLog(@"Ezfun:webview; webViewDidFinishLoad in");
    NSString* url = [webView stringByEvaluatingJavaScriptFromString:@"document.URL"];
    UnitySendMessage( [gameObjectName UTF8String], "onLoadFinish", [url UTF8String] );
    NSLog(@"Ezfun:webview; webViewDidFinishLoad in");
}
- (void)webViewDidFailLoadWithError: (NSError*)error
{
    NSLog(@"Ezfun:webview; webViewDidFailLoadWithError");
    [UIApplication sharedApplication].networkActivityIndicatorVisible = NO;
    
    NSInteger err_code = [error code];
    if( err_code == NSURLErrorCancelled )
    {
        return;
    }
    
}

- (void)setMargins:(int)left top:(int)top right:(int)right bottom:(int)bottom
{
    UIView *view = UnityGetGLViewController().view;
    
    //CGRect frame = view.frame;
    CGRect frame;
    
    if ([[UIScreen mainScreen] bounds].size.width > [[UIScreen mainScreen] bounds].size.height)
    {
        frame = CGRectMake(0, 0, [[UIScreen mainScreen] bounds].size.width, [[UIScreen mainScreen] bounds].size.height);
    }
    else
    {
        frame = CGRectMake(0, 0, [[UIScreen mainScreen] bounds].size.height, [[UIScreen mainScreen] bounds].size.width);
    }
    
    //NSLog(@"Ezfun:webview; set rect boundry (%d,%d,%d,%d).", (int)left, (int)top, (int)right, (int)bottom);
    //NSLog(@"Ezfun:webview; screen rect:origin (%d,%d), size(%d,%d).", (int)frame.origin.x, (int)frame.origin.y, (int)frame.size.width, (int)frame.size.height);
    
    CGFloat scale = view.contentScaleFactor;
    //NSLog(@"Ezfun:webview; scale %f", scale);
    frame.size.width -= (left + right) / scale;
    frame.size.height -= (top + bottom) / scale;
    frame.origin.x += left / scale;
    frame.origin.y += top / scale;
    //NSLog(@"Ezfun:webview; show rect: origin (%d,%d), size(%d,%d).", (int)frame.origin.x, (int)frame.origin.y, (int)frame.size.width, (int)frame.size.height);
    webView.frame = frame;
}

- (void)setVisibility:(BOOL)visibility
{
    webView.hidden = visibility ? NO : YES;
    [webView setUserInteractionEnabled:visibility ? YES : NO];
}

- (void)loadURL:(const char *)url
{
    NSLog(@"Ezfun:webview; loadURL %s", url);
    //NSString *urlBase = [NSString stringWithCString:url encoding:NSUTF8StringEncoding];
    //urlBase = [urlBase stringByAddingPercentEscapesUsingEncoding:NSUTF8StringEncoding];
    //NSURL *nsurl = [NSURL URLWithString:urlBase ];
	NSString *urlStr = [NSString stringWithUTF8String:url];
    NSURL *nsurl = [NSURL URLWithString:urlStr ];
    NSURLRequest *request = [NSURLRequest requestWithURL:nsurl];
    [webView loadRequest:request];
    //[webView reload];
}

- (void)evaluateJS:(const char *)js
{
    NSString *jsStr = [NSString stringWithUTF8String:js];
    [webView stringByEvaluatingJavaScriptFromString:jsStr];
}

@end

extern "C" {
    void *_WebViewPlugin_Init(const char *gameObjectName);
    void _WebViewPlugin_Destroy(void *instance);
    void _WebViewPlugin_SetMargins(
                                   void *instance, int left, int top, int right, int bottom);
    void _WebViewPlugin_SetVisibility(void *instance, BOOL visibility);
    void _WebViewPlugin_LoadURL(void *instance, const char *url);
    void _WebViewPlugin_EvaluateJS(void *instance, const char *url);
}

void *_WebViewPlugin_Init(const char *gameObjectName)
{
    id instance = [[WebViewPlugin alloc] initWithGameObjectName:gameObjectName];
    return (__bridge void *)instance;
}

void _WebViewPlugin_Destroy(void *instance)
{
    WebViewPlugin *webViewPlugin = (__bridge WebViewPlugin *)instance;
    //[webViewPlugin release];
}

void _WebViewPlugin_SetMargins(
                               void *instance, int left, int top, int right, int bottom)
{
    WebViewPlugin *webViewPlugin = (__bridge WebViewPlugin *)instance;
    [webViewPlugin setMargins:left top:top right:right bottom:bottom];
}

void _WebViewPlugin_SetVisibility(void *instance, BOOL visibility)
{
    WebViewPlugin *webViewPlugin = (__bridge WebViewPlugin *)instance;
    [webViewPlugin setVisibility:visibility];
}

void _WebViewPlugin_LoadURL(void *instance, const char *url)
{
    WebViewPlugin *webViewPlugin = (__bridge WebViewPlugin *)instance;
    [webViewPlugin loadURL:url];
}

void _WebViewPlugin_EvaluateJS(void *instance, const char *js)
{
    WebViewPlugin *webViewPlugin = (__bridge WebViewPlugin *)instance;
    [webViewPlugin evaluateJS:js];
}
