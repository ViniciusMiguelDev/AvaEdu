# Definition of Done (DoD)

This document defines the criteria that must be met before any work item (user story, task, or bug fix) can be considered "Done" and ready for release.

## Universal Definition of Done

All work items must meet these criteria regardless of type:

### Code Quality
- [ ] **Code Complete:** All code is written and implements the required functionality
- [ ] **Code Standards:** Code follows established coding conventions and style guides
- [ ] **Code Review:** Code has been reviewed and approved by at least one peer
- [ ] **No Code Smells:** No obvious code quality issues (duplicated code, long methods, etc.)
- [ ] **Performance:** Code meets performance requirements and standards
- [ ] **Security:** Security best practices are implemented and verified

### Testing
- [ ] **Unit Tests:** Unit tests are written and achieve minimum coverage (≥80%)
- [ ] **Unit Tests Passing:** All unit tests pass locally and in CI/CD pipeline
- [ ] **Integration Tests:** Integration tests are written where applicable
- [ ] **Integration Tests Passing:** All integration tests pass
- [ ] **Manual Testing:** Manual testing has been performed and documented
- [ ] **Edge Cases:** Edge cases and error scenarios have been tested
- [ ] **Regression Testing:** Existing functionality has not been broken

### Documentation
- [ ] **Code Documentation:** Code is properly commented and self-documenting
- [ ] **API Documentation:** API changes are documented (if applicable)
- [ ] **User Documentation:** End-user documentation is updated (if applicable)
- [ ] **Technical Documentation:** Technical specifications are updated
- [ ] **Deployment Notes:** Any deployment or configuration changes are documented

### Quality Assurance
- [ ] **Acceptance Criteria Met:** All acceptance criteria are satisfied
- [ ] **Functional Requirements:** All functional requirements are implemented
- [ ] **Non-Functional Requirements:** Performance, security, usability requirements met
- [ ] **Browser/Device Testing:** Tested on required browsers/devices (if web/mobile)
- [ ] **Accessibility:** Accessibility requirements are met (if applicable)

### Process Compliance
- [ ] **Code Repository:** Code is committed to version control
- [ ] **Branch Management:** Feature branch is merged to appropriate target branch
- [ ] **Build Pipeline:** All automated builds and checks pass
- [ ] **Deployment Ready:** Code is ready for deployment to next environment
- [ ] **Stakeholder Approval:** Product Owner or stakeholder approval received

## Type-Specific Definitions

### User Stories
Additional criteria for user stories:

- [ ] **User Value:** Delivers measurable value to end users
- [ ] **Demo Ready:** Can be demonstrated to stakeholders
- [ ] **User Acceptance:** User acceptance testing completed (if required)
- [ ] **Analytics:** Tracking/analytics implemented for success metrics (if applicable)

### Bug Fixes
Additional criteria for bug fixes:

- [ ] **Root Cause:** Root cause has been identified and addressed
- [ ] **Verification:** Original bug scenario has been tested and resolved
- [ ] **Regression Prevention:** Tests added to prevent regression
- [ ] **Impact Assessment:** Impact on other system components verified

### Technical Tasks
Additional criteria for technical debt and infrastructure work:

- [ ] **Technical Validation:** Technical implementation verified by senior developer
- [ ] **Performance Impact:** Performance impact measured and acceptable
- [ ] **System Integration:** Integration with existing systems verified
- [ ] **Rollback Plan:** Rollback procedure documented and tested (if applicable)

### Documentation Tasks
Additional criteria for documentation work:

- [ ] **Content Review:** Content reviewed for accuracy and completeness
- [ ] **Grammar/Style:** Grammar and style checked and corrected
- [ ] **Stakeholder Review:** Reviewed by relevant stakeholders
- [ ] **Accessibility:** Documentation is accessible and well-organized

## Environment-Specific Criteria

### Development Environment
- [ ] Code works correctly in local development environment
- [ ] All development dependencies are properly configured
- [ ] Local tests pass consistently

### Staging/Test Environment
- [ ] Deployed successfully to staging environment
- [ ] End-to-end tests pass in staging
- [ ] Performance testing completed (if applicable)
- [ ] Security scanning completed (if applicable)

### Production Environment
- [ ] Deployment procedure documented and reviewed
- [ ] Rollback procedure available and tested
- [ ] Monitoring and alerting configured
- [ ] Production deployment approved by authorized personnel

## Quality Gates

### Code Quality Gates
- **Test Coverage:** Minimum 80% code coverage
- **Cyclomatic Complexity:** Maximum complexity of 10 per method
- **Code Duplication:** Less than 5% duplicated code
- **Technical Debt:** No new critical or high-severity technical debt

### Performance Gates
- **Response Time:** API responses under 200ms for 95th percentile
- **Page Load Time:** Web pages load under 3 seconds
- **Memory Usage:** Memory consumption within acceptable limits
- **Database Performance:** Database queries optimized and indexed

### Security Gates
- **Vulnerability Scanning:** No high or critical security vulnerabilities
- **Authentication:** Proper authentication and authorization implemented
- **Data Protection:** Sensitive data properly encrypted and handled
- **Input Validation:** All inputs validated and sanitized

## Exceptions and Waivers

### When DoD Can Be Modified
- **Hotfixes:** Critical production issues may have abbreviated DoD
- **Spikes/Research:** Exploratory work may have different criteria
- **Documentation-Only:** Pure documentation changes may skip some technical criteria

### Waiver Process
1. **Request:** Document specific DoD items that cannot be met
2. **Justification:** Provide clear business justification
3. **Risk Assessment:** Document risks of not meeting DoD
4. **Approval:** Get approval from Product Owner and Tech Lead
5. **Tracking:** Track waived items for future completion

## Monitoring and Improvement

### DoD Compliance Metrics
- Percentage of work items meeting DoD on first review
- Number of post-release defects per work item
- Time to resolve DoD violations
- Frequency of DoD waivers

### Continuous Improvement
- Review DoD quarterly with the team
- Gather feedback on practicality and effectiveness
- Update criteria based on lessons learned
- Align with industry best practices

### Team Education
- Regular DoD training for new team members
- DoD review sessions during retrospectives
- Share examples of good and poor DoD compliance
- Celebrate teams and individuals who consistently meet DoD

## Tools and Automation

### Automated Checks
- [ ] Automated code quality analysis (SonarQube, CodeClimate, etc.)
- [ ] Automated security scanning (SAST/DAST tools)
- [ ] Automated test execution in CI/CD pipeline
- [ ] Automated deployment validation

### Manual Verification
- [ ] Code review process
- [ ] Manual testing procedures
- [ ] Documentation review process
- [ ] Stakeholder approval workflow

## Responsibilities

### Developer Responsibilities
- Understand and follow DoD criteria
- Perform self-assessment before submitting work
- Address any DoD violations promptly
- Seek clarification when criteria are unclear

### Reviewer Responsibilities
- Verify DoD compliance during code review
- Provide constructive feedback on DoD violations
- Approve work only when DoD is satisfied
- Escalate recurring DoD issues

### Team Lead Responsibilities
- Ensure team understands DoD
- Monitor DoD compliance metrics
- Address systemic DoD issues
- Update DoD based on team feedback

### Product Owner Responsibilities
- Understand DoD impact on delivery timeline
- Approve DoD waivers when necessary
- Provide feedback on user-facing DoD criteria
- Balance quality requirements with delivery needs

---

*Definition of Done Version: 1.0*
*Last Updated: December 1, 2025*
*Next Review Date: March 1, 2026*
*Document Owner: Development Team*