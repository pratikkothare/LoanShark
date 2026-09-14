import { HttpClient } from '@angular/common/http';
import {
    Component,
    ViewChild,
    ElementRef
} from '@angular/core';

@Component({
    selector: 'app-chatbot',
    templateUrl: './chatbot.component.html',
    styleUrls: ['./chatbot.component.css']
})
export class ChatbotComponent {

    showChat = false;
    isTyping = false;
    userMessage = '';
    showLoginButton = false;

    eligibilityStep = 0;

    tempData = {
        income: 0,
        amount: 0,
        cibil: 0,
        tenure: 0
    };

    @ViewChild('chatBody')
    chatBody!: ElementRef;

    messages: any[] = [];

    constructor(private http: HttpClient) {

        const user = localStorage.getItem('user');

        if (user) {

            const savedChat = localStorage.getItem('loaniq_chat');

            if (savedChat) {
                this.messages = JSON.parse(savedChat);
            }
            else {
                this.messages = [{
                    sender: 'bot',
                  text: 'Hello 👋 Ask me about loan eligibility, EMI or documents.\n' +
                    '• Check eligibility\n' +
                    '• Show loan products\n' +
                    '• Required documents\n' +
                    '• Loan Applications'
                }];
            }

        } else {

            this.messages = [{
                sender: 'bot',
              text: 'Hello 👋 Ask me about loan eligibility, EMI or documents.\n'+
                    '• Check eligibility\n' +
                '• Show loan products\n' +
                '• Required documents\n' +
                '• Loan Applications'
            }];

        }

    }

    toggleChat() {
        this.showChat = !this.showChat;

        if (this.showChat) {
            setTimeout(() => {
                this.scrollToBottom();
            }, 300);
        }
    }

    scrollToBottom() {
        setTimeout(() => {
            if (this.chatBody) {
                this.chatBody.nativeElement.scrollTop =
                    this.chatBody.nativeElement.scrollHeight;
            }
        }, 100);
    }

    private saveChat() {

        const user = localStorage.getItem('user');

        if (!user) {
            return;
        }

        localStorage.setItem(
            'loaniq_chat',
            JSON.stringify(this.messages)
        );
    }

  private addBotMessage(msg: any) {
    this.isTyping = true;
    const delay = Math.floor(Math.random() * 1200) + 800;
    setTimeout(() => {
      this.isTyping = false;

      this.messages.push(msg);
      this.saveChat();
      this.scrollToBottom();
    }, delay);
       
    }

    sendMessage() {

        if (!this.userMessage.trim()) return;

        let originalMessage = this.userMessage.trim();
        let prompt = originalMessage.toLowerCase();

        this.messages.push({
            sender: 'user',
            text: originalMessage
        });

        this.saveChat();
        this.scrollToBottom();

        this.userMessage = '';


        if (this.eligibilityStep > 0) {
            this.processEligibility(originalMessage);
            return;
        }


        if (
            prompt === 'hi' ||
            prompt === 'hello' ||
            prompt === 'hey'
        ) {
            this.addBotMessage({
                sender: 'bot',
              text: 'Hello 👋 How can I help with your loan applications today?\n'+
                    '• Check eligibility\n' +
                    '• Show loan products\n' +
                    '• Required documents\n' +
                    '• Loan Applications'
            });
            return;
        }

      if (
        prompt === 'thanks' ||
        prompt === 'thankyou' ||
        prompt === 'thank you' ||
        prompt === 'Thank you'
      ) {
        this.addBotMessage({
          sender: 'bot',
          text: 'Happy to Serve you!  Do you have any other query?'
        });
        return;
      }

        if (
            prompt.includes('eligibility') ||
            prompt.includes('eligible')
        ) {
            this.eligibilityStep = 1;

            this.addBotMessage({
                sender: 'bot',
                text: 'I can help with that. What is your Monthly Income?'
            });

            return;
        }


        if (
            prompt.includes('process') ||
            prompt.includes('steps') ||
            prompt.includes('how to apply')
        ) {

            this.addBotMessage({
                sender: 'bot',
                text:
                    'Loan application process:\n\n' +
                    '1. Select loan type\n' +
                    '2. Check eligibility\n' +
                    '3. Calculate EMI\n' +
                    '4. Upload documents\n' +
                    '5. Submit application\n' +
                    '6. Admin verifies documents\n' +
                    '7. Loan gets sanctioned'
            });

            return;
        }


        if (
            prompt.includes('documents') ||
            prompt.includes('docs')
        ) {

            this.addBotMessage({
                sender: 'bot',
                text:
                    'Required documents:\n' +
                    '1. Aadhaar Card\n' +
                    '2. PAN Card\n' +
                    '3. Bank Statement\n' +
                    '4. Salary Slips'
            });

            return;
        }


        if (
            prompt.includes('withdraw') ||
            prompt.includes('cancel application')
        ) {
            this.handleWithdrawRequest();
            return;
        }


        const loanKeywords = [
            'application',
            'applications',
            'status',
            'applied'
        ];

        const isLoanQuery =
            loanKeywords.some(k => prompt.includes(k));

        if (isLoanQuery) {
            this.handleLoanStatusQuery();
            return;
        }


        if (
            prompt.includes('product') ||
            prompt.includes('offers') ||
            prompt.includes('loan types')
        ) {
            this.handleLoanProductsQuery();
            return;
        }


        if (
            prompt.includes('name') ||
            prompt.includes('who are you')
        ) {
            this.addBotMessage({
                sender: 'bot',
                text: 'I am LoanIQ AI Assistant'
            });
            return;
      }

      if (
        prompt.includes('apply') ||
        prompt.includes('loan apply') ||
        prompt.includes('home loan') ||
        prompt.includes('car loan') ||
        prompt.includes('personal loan')
      ) {

        this.addBotMessage({
          sender: 'bot',
          text: 'You can apply for a loan from here: ',
          showLoanProducts: true
        });

        return;
      }


        this.addBotMessage({
            sender: 'bot',
            text:
                'I am still learning, Try asking:\n' +
                '• Check eligibility\n' +
                '• Show loan products\n' +
                '• Required documents\n' +
                '• Loan Application'
        });

    }


    private processEligibility(input: string) {

        const val = parseFloat(input);

        if (isNaN(val) || val <= 0) {
            this.addBotMessage({
                sender: 'bot',
                text: 'Please enter valid number.'
            });
            return;
        }

        if (this.eligibilityStep === 1) {

            this.tempData.income = val;
            this.eligibilityStep = 2;

            this.addBotMessage({
                sender: 'bot',
                text: 'Enter desired Loan Amount?'
            });

        }

        else if (this.eligibilityStep === 2) {

            this.tempData.amount = val;
            this.eligibilityStep = 3;

            this.addBotMessage({
                sender: 'bot',
                text: 'Enter your CIBIL score?'
            });

        }

        else if (this.eligibilityStep === 3) {

          const cibil = val;

          if (cibil < 300 || cibil > 900) {
            this.addBotMessage({
              sender: 'bot',
              text: '⚠️ CIBIL score must be between 300 and 900. Please enter again.'
            });
            return;
          }

          this.tempData.cibil = cibil;
          this.eligibilityStep = 4;

          this.addBotMessage({
            sender: 'bot',
            text: 'Enter tenure in months?'
          });

        }

        else if (this.eligibilityStep === 4) {

            this.tempData.tenure = val;

            this.runCalculation();

            this.eligibilityStep = 0;
        }

    }

  private runCalculation() {

    const { income, amount, cibil, tenure } = this.tempData;

    const emi = amount / tenure;

    const eligible =
      cibil > 650 &&
      emi <= (income * .5);

    const user = localStorage.getItem('user');

    if (eligible) {

     
      this.addBotMessage({
        sender: 'bot',
        text: 'You are Eligible for loan.'
      });

      if (!user) {
        this.addBotMessage({
          sender: 'bot',
          text: 'For further processing please login.',
          showLoginButton: true
        });
      }


    } else {

      let reasons: string[] = [];

      if (cibil <= 650) {
        reasons.push('Low CIBIL score (should be above 650)');
      }

      if (emi > (income * 0.5)) {
        reasons.push('EMI exceeds 50% of your monthly income');
      }

      let reasonText = reasons.length
        ? reasons.join(' and ')
        : 'criteria not met';

      this.addBotMessage({
        sender: 'bot',
        text: `You are not eligible currently.\nReason: ${reasonText}`
      });
    }
  }


    private handleLoanStatusQuery() {

        let user = localStorage.getItem('user');

        if (!user) {

            this.addBotMessage({
                sender: 'bot',
                text: 'Please login first.',
                showLoginButton: true
            });

            return;
        }

        let userObj = JSON.parse(user);
        let userId = userObj.userId;

        this.http.get<any>(
            `https://localhost:7156/api/Loan/getloans/${userId}`
        ).subscribe({

            next: (res) => {

                if (!res.data?.length) {

                    this.addBotMessage({
                        sender: 'bot',
                        text: 'You have not applied for any loan yet.'
                    });

                } else {

                    this.addBotMessage({
                        sender: 'bot',
                        text: 'APPLICATION_CARDS',
                        applications: JSON.parse(
                            JSON.stringify(res.data)
                        )
                    });

                }

            },

            error: () => {
                this.showErrorMessage(
                    'Could not fetch applications.'
                );
            }

        });

    }


    private handleLoanProductsQuery() {

        this.isTyping = true;

        this.http.get<any>(
            'https://localhost:7156/api/Loan/products'
        ).subscribe({

            next: (res) => {

                this.isTyping = false;

                if (res.success && res.data?.length) {

                    this.addBotMessage({
                        sender: 'bot',
                        text: 'We offer following products:',
                        products: JSON.parse(
                            JSON.stringify(res.data)
                        )
                    });

                } else {

                    this.addBotMessage({
                        sender: 'bot',
                        text: 'No products listed.'
                    });

                }

            },

            error: () => {
                this.isTyping = false;
                this.showErrorMessage(
                    'Unable to fetch loan products.'
                );
            }

        });

    }


    private handleWithdrawRequest() {

        let user = localStorage.getItem('user');

        if (!user) {

            this.addBotMessage({
                sender: 'bot',
                text: 'Please login first.',
                showLoginButton: true
            });

            return;
        }

        let userObj = JSON.parse(user);
        let userId = userObj.userId;

        this.http.get<any>(
            `https://localhost:7156/api/Loan/getloans/${userId}`
        ).subscribe({

            next: (res) => {

                let pendingApps =
                    res.data.filter(
                        (x: any) => x.status === 'Pending'
                    );

                if (!pendingApps.length) {

                    this.addBotMessage({
                        sender: 'bot',
                        text: 'No pending applications to withdraw.'
                    });

                    return;
                }

                this.addBotMessage({
                    sender: 'bot',
                    text: 'Select application to withdraw:',
                    withdrawOptions: JSON.parse(
                        JSON.stringify(pendingApps)
                    )
                });

            },

            error: () => {
                this.showErrorMessage(
                    'Unable to fetch applications.'
                );
            }

        });

    }


    withdrawApplication(applicationId: number) {

        this.http.post<any>(
            'https://localhost:7156/api/Loan/withdraw',
            applicationId
        ).subscribe({

            next: (res) => {
                this.addBotMessage({
                    sender: 'bot',
                    text: res.message
                });
            },

            error: () => {
                this.showErrorMessage(
                    'Unable to withdraw application.'
                );
            }

        });

    }


    private showErrorMessage(errorText: string) {
        this.addBotMessage({
            sender: 'bot',
            text: errorText
        });
    }

    closeChat() {
        this.showChat = false;
    }

    goToLogin() {
        window.location.href = 'http://localhost:4200/login';
   }
    goToLoanProducts() {
      window.location.href = 'http://localhost:4200/loanproducts';
    }

}
