# OCBCJointAccountCreation
# Background
This app was made as part of our Portfolio Development module in collaboration with OCBC. 
It was designed in accordance to a challenge statement set by OCBC entitled, "SAFE AND SEAMLESS JOINT-ACCOUNT CREATION".
This challenge required participants to develop a solution to create a seamless and secured method to allow the Bank
to perform a successful digital joint account opening application. 

# Workflow
<!-- wp:paragraph -->
<p>The application process is as such:</p>
<!-- /wp:paragraph -->

<!-- wp:list -->
<ul><li>The main applicant selects whether to log in with Singpass, his existing OCBC iBanking account or create a new account (if he is a new-to-bank customer).<ul><li>For new-to-bank customers/those without Singpass or iBanking, they would be required to upload an images of documents (NRIC, foreign pass, passport) to prove their identity</li><li>Their information such as name, NRIC no and address will be extracted from these images using an Optical Character Recognition (OCR) API</li><li>We will also call an API to access their device's camera so that the applicant can take a selfie. This will be cross-checked with the picture of their face shown on the uploaded documents using another facial verification API to ensure the documents are theirs</li></ul></li><li>Once logged in, the main applicant will enter the joint applicant's details (name, email address and contact no.) including their NRIC number. This is to ensure the main and joint applicants know and trust each other and are applying consensually.</li><li>After this, the main applicant will be asked to verify that all information entered is correct before clicking submit. Once submitted, the joint applicant will receive a uniquely generated link sent through email and SMS that would take allow them to continue the application process in their own time</li><li>When the joint applicant clicks on this link, the same steps will repeat. However, after he/she verifies the information entered, a screen stating whether the application was successful will appear. If successful, the account will be opened.</li></ul>
<!-- /wp:list -->
