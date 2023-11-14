import UIKit
import WebKit

final class ReCAPTCHAViewController: UIViewController {
    private var webView: WKWebView!
    private let viewModel: ReCAPTCHAViewModel

    init(viewModel: ReCAPTCHAViewModel) {
        self.viewModel = viewModel

        super.init(nibName: nil, bundle: nil)
    }

    required init?(coder: NSCoder) {
        fatalError("init(coder:) has not been implemented")
    }

    override func loadView() {
        let webConfiguration = WKWebViewConfiguration()
        let contentController = WKUserContentController()

        contentController.add(viewModel, name: "recaptcha")
        webConfiguration.userContentController = contentController

        webView = WKWebView(frame: .zero, configuration: webConfiguration)
        webView!.isOpaque = false
        webView!.backgroundColor = UIColor.clear
        webView!.scrollView.backgroundColor = UIColor.clear
        view = webView
    }

    override func viewDidLoad() {
        super.viewDidLoad()

        navigationItem.leftBarButtonItem = UIBarButtonItem(
            barButtonSystemItem: .stop,
            target: self,
            action: #selector(didSelectCloseButton)
        )

        webView.loadHTMLString(viewModel.html, baseURL: viewModel.url)
    }
}

// MARK: - Target-Actions
private extension ReCAPTCHAViewController {
    @IBAction func didSelectCloseButton() {
        viewModel.delegate?.didFailed(message: "")
        dismiss(animated: true)
    }
}
