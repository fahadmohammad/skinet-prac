export interface exAuthResponseDto {
    isAuthSuccessful: boolean;
    errorMessage: string;
    token: string;
    is2StepVerificationRequired: boolean;
    provider: string;
}