# BMAD Project Quick Start Guide

Welcome to the BMAD project! This guide will help you get started with our workflow and documentation system.

## 📁 Project Structure

```
BMAD/
├── .agentvibes/          # Agent configuration
├── .bmad/                # Core BMAD configuration  
├── .claude/              # Claude AI integration settings
├── docs/
│   ├── sprint-artifacts/ # Sprint planning & tracking
│   ├── technical/        # API docs, guides, schemas
│   ├── architecture/     # Architecture decisions (ADRs)
│   └── workflows/        # Process documentation
└── WORKFLOW.md           # Main workflow overview
```

## 🚀 Getting Started

### 1. Workflow Overview
Read the main [`WORKFLOW.md`](./WORKFLOW.md) to understand our project management approach.

### 2. Sprint Management
- Use templates in [`docs/sprint-artifacts/`](./docs/sprint-artifacts/) for sprint planning
- Follow the sprint planning template for consistent sprint organization
- Document retrospectives using the provided template

### 3. Task Management
- Reference [`task-management-workflow.md`](./docs/workflows/task-management-workflow.md) for task lifecycle
- Use the bug report template for consistent issue tracking
- Follow the defined task states: Backlog → In Progress → Code Review → Testing → Done

### 4. Development Process
- Follow the [Code Review Checklist](./docs/workflows/code-review-checklist.md) before submitting code
- Ensure all work meets our [Definition of Done](./docs/workflows/definition-of-done.md)
- Use architecture decision records (ADRs) for significant technical decisions

## 📚 Available Templates

### Sprint Management
- **Sprint Planning Template** - Plan and organize sprints
- **User Story Template** - Document user stories with acceptance criteria
- **Retrospective Template** - Conduct effective sprint retrospectives

### Development
- **Bug Report Template** - Standardized bug reporting
- **Code Review Checklist** - Ensure quality code reviews
- **Definition of Done** - Quality criteria for all work

### Technical Documentation
- **API Endpoint Template** - Document API endpoints consistently
- **Architecture Decision Record (ADR) Template** - Record important architectural decisions

## 🔄 Daily Workflow

1. **Morning Standup**
   - Review current sprint progress
   - Update task status in tracking system
   - Identify blockers and dependencies

2. **Development Work**
   - Follow task management workflow
   - Adhere to code review checklist
   - Ensure definition of done is met

3. **End of Day**
   - Update task progress
   - Commit and push code changes
   - Document any blockers or issues

## 📝 Documentation Guidelines

### Creating New Documentation
1. Choose appropriate template from `docs/technical/` or `docs/workflows/`
2. Follow naming convention: `kebab-case.md`
3. Include proper headers and metadata
4. Get peer review before finalizing

### Updating Existing Documentation
1. Make changes in appropriate files
2. Update "Last Updated" dates
3. Add entry to changelog if applicable
4. Notify team of significant changes

## 🛠️ Tools and Integrations

### Configuration Management
- **`.bmad/`** - Core project configuration
- **`.agentvibes/`** - Agent behavior settings
- **`.claude/`** - AI integration configuration

### Development Tools
- Version control with proper branching strategy
- Automated testing and CI/CD pipelines
- Code quality and security scanning tools

## 📊 Key Metrics

We track the following to improve our processes:
- Sprint velocity and completion rates
- Code review cycle times
- Bug resolution times
- Definition of Done compliance
- Team satisfaction and feedback

## 🆘 Getting Help

### Process Questions
- Check relevant documentation in `docs/workflows/`
- Ask team lead or experienced team members
- Raise in team standup or retrospective

### Technical Questions
- Review technical documentation in `docs/technical/`
- Check architecture decision records
- Consult with senior developers or architects

### Project Questions
- Review project overview and sprint artifacts
- Contact Product Owner for requirements clarification
- Use established communication channels

## 🔄 Continuous Improvement

Our workflow is designed to evolve:
- Regular retrospectives to identify improvements
- Quarterly documentation reviews
- Team feedback integration
- Process metrics analysis

## 📅 Important Dates

- **Sprint Planning:** [Define schedule]
- **Daily Standups:** [Define schedule]  
- **Sprint Reviews:** [Define schedule]
- **Retrospectives:** [Define schedule]
- **Documentation Reviews:** Quarterly

---

**Ready to Start?**
1. Read through this guide
2. Review the main WORKFLOW.md
3. Check current sprint artifacts
4. Join the team for the next standup
5. Start contributing!

*Last Updated: December 1, 2025*
*Questions? Contact: [Team Lead/Project Manager]*