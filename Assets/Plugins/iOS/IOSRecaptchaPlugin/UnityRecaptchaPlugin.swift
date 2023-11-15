import UIKit

@objc public class UnityRecaptchaPlugin: UIViewController {
    private static var reCAPTCHAViewModel: ReCAPTCHAViewModel?
    private static var reCAPTCHAViewController: ReCAPTCHAViewController?
    private static var siteKey: String?

    @objc public static func recaptchaVerify(recaptchaSiteKey: String) {
        let parentViewController = UnityFramework.getInstance()?.appController()?.rootViewController
        
        siteKey = recaptchaSiteKey
        
        //let childViewController = ReCAPTCHAViewController()
        let childViewController = UnityRecaptchaPlugin()
        
        parentViewController?.addChild(childViewController)

        parentViewController?.view.addSubview(childViewController.view)
        
        childViewController.view.backgroundColor = UIColor.black.withAlphaComponent(0.1)
/*
        childViewController.view.frame = parentViewController?.view?.bounds
        childViewController.view.autoresizingMask = [.flexibleWidth, .flexibleHeight]

        childViewController.didMove(toParent: parentViewController)
        
        UIView.transition(with: parentViewController?.view, duration: 0.2, options: .transitionCrossDissolve, animations: {
            popupContainer.isHidden = false
        })
 */
    }
    
    override public func viewDidLoad() {
        super.viewDidLoad()
        let viewModel = ReCAPTCHAViewModel(
            siteKey: UnityRecaptchaPlugin.siteKey!,
            url: URL(string: "https://openlogin.portkey.finance")!
        )

        viewModel.delegate = self

        let vc = ReCAPTCHAViewController(viewModel: viewModel)
        UnityRecaptchaPlugin.reCAPTCHAViewController = vc

        // Optional: present the ReCAPTCHAViewController so you have a navigation bar
        let nav = UINavigationController(rootViewController: vc)

        // Keep a reference to the View Model so we can be alerted when the user
        // solves the CAPTCHA.
        //reCAPTCHAViewModel = viewModel

        present(nav, animated: true)
    }
}

// MARK: - ReCAPTCHAViewModelDelegate
extension UnityRecaptchaPlugin: ReCAPTCHAViewModelDelegate {
    func didSolveCAPTCHA(token: String) {
        UnityFramework.getInstance().sendMessageToGO(withName: "IOSCaptchaCallback", functionName: "OnCaptchaSuccess", message: token)
        UnityRecaptchaPlugin.reCAPTCHAViewController?.dismiss(animated: true)
    }
    
    func didFailed(message: String) {
        UnityFramework.getInstance().sendMessageToGO(withName: "IOSCaptchaCallback", functionName: "OnCaptchaError", message: message)
    }
}
