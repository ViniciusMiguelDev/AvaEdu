# Code Review Checklist

## Pre-Review Checklist (Author)

Before submitting code for review, ensure the following items are completed:

### Code Quality
- [ ] Code follows established coding standards and conventions
- [ ] Variable and function names are descriptive and meaningful
- [ ] Code is properly formatted and indented
- [ ] No commented-out code or debug statements remain
- [ ] Complex logic is properly commented and documented
- [ ] No hardcoded values (use configuration/constants instead)

### Functionality
- [ ] Implementation meets all acceptance criteria
- [ ] Edge cases and error conditions are handled
- [ ] Input validation is implemented where appropriate
- [ ] Performance considerations have been addressed
- [ ] Security best practices are followed

### Testing
- [ ] Unit tests are written and passing
- [ ] Test coverage meets project standards (minimum X%)
- [ ] Integration tests are added if applicable
- [ ] Manual testing has been performed
- [ ] All existing tests still pass

### Documentation
- [ ] Code comments are clear and helpful
- [ ] API documentation is updated (if applicable)
- [ ] README or technical docs are updated if needed
- [ ] Inline documentation follows project standards

### Dependencies
- [ ] No unnecessary dependencies added
- [ ] New dependencies are approved and documented
- [ ] Dependency versions are pinned appropriately
- [ ] License compatibility verified for new dependencies

## Code Review Checklist (Reviewer)

### Initial Review
- [ ] Pull request description clearly explains the changes
- [ ] Changes align with the stated requirements
- [ ] Scope of changes is appropriate (not too large)
- [ ] No unrelated changes included

### Code Analysis

#### Architecture & Design
- [ ] Code follows established architectural patterns
- [ ] Design decisions are appropriate for the problem
- [ ] Code is modular and follows separation of concerns
- [ ] No violation of SOLID principles or design patterns
- [ ] Database schema changes are properly designed

#### Code Quality
- [ ] Code is readable and maintainable
- [ ] Functions/methods have single responsibility
- [ ] Appropriate abstraction levels are used
- [ ] No code duplication (DRY principle followed)
- [ ] Error handling is comprehensive and appropriate

#### Performance
- [ ] No obvious performance bottlenecks
- [ ] Database queries are optimized
- [ ] Memory usage is efficient
- [ ] Async operations are used appropriately
- [ ] Caching strategies are implemented where beneficial

#### Security
- [ ] Input validation and sanitization are proper
- [ ] Authentication and authorization are correctly implemented
- [ ] No sensitive data is logged or exposed
- [ ] SQL injection vulnerabilities are prevented
- [ ] Cross-site scripting (XSS) prevention is in place

### Testing Review
- [ ] Test cases cover happy path scenarios
- [ ] Edge cases and error conditions are tested
- [ ] Test names are descriptive and meaningful
- [ ] Tests are independent and don't rely on external state
- [ ] Mocking is used appropriately for dependencies

### Documentation Review
- [ ] Code comments explain "why" not just "what"
- [ ] Public APIs are properly documented
- [ ] Complex algorithms or business logic are explained
- [ ] Configuration changes are documented

### Deployment Considerations
- [ ] Changes are backward compatible (if required)
- [ ] Database migrations are safe and reversible
- [ ] Feature flags are used for risky changes (if applicable)
- [ ] Environment-specific configurations are handled

## Review Process Guidelines

### For Authors
1. **Self-Review First:** Review your own code before submitting
2. **Small Changes:** Keep pull requests focused and reasonably sized
3. **Clear Description:** Provide context and explanation for changes
4. **Address Feedback:** Respond to comments and questions promptly
5. **Update Tests:** Ensure tests reflect the changes made

### For Reviewers
1. **Timely Reviews:** Complete reviews within [X business hours/days]
2. **Constructive Feedback:** Provide specific, actionable comments
3. **Ask Questions:** Don't hesitate to ask for clarification
4. **Test Locally:** Run the code locally when appropriate
5. **Approve Thoughtfully:** Only approve when fully satisfied

### Review Comments Guidelines

#### Types of Comments
- **Must Fix:** Critical issues that must be addressed before merge
- **Should Fix:** Important improvements that should be made
- **Consider:** Suggestions for improvement (optional)
- **Question:** Requests for clarification or explanation
- **Nitpick:** Minor style or preference issues (optional)

#### Comment Examples

**Good Comments:**
- "This function could cause a performance issue with large datasets. Consider pagination."
- "What happens if `userId` is null here? Should we add validation?"
- "Great solution! This is much cleaner than the previous approach."

**Avoid:**
- "This is wrong." (not specific enough)
- "Just use X instead." (no explanation)
- "I don't like this." (not constructive)

## Definition of Done for Code Review

A code review is considered complete when:

- [ ] All review comments have been addressed or discussed
- [ ] At least [X] reviewer(s) have approved the changes
- [ ] All automated checks (CI/CD) are passing
- [ ] No blocking issues remain unresolved
- [ ] Documentation has been updated as needed

## Review Metrics to Track

### Individual Metrics
- Review completion time
- Number of issues found per review
- Comment resolution time

### Team Metrics
- Average review cycle time
- Review participation rate
- Post-merge defect rate

## Tools and Resources

### Review Tools
- [Code review platform/tool]
- [Static analysis tools]
- [Automated testing results]

### Reference Materials
- [Coding standards document]
- [Architecture guidelines]
- [Security best practices]

## Escalation Process

If there are disagreements during code review:

1. **Discussion:** Try to reach consensus through discussion
2. **Team Lead:** Involve team lead for technical decisions
3. **Architecture Review:** Escalate architectural concerns to senior team
4. **Documentation:** Document decisions for future reference

---

*Checklist Version: 1.0*
*Last Updated: December 1, 2025*
*Process Owner: Development Team*