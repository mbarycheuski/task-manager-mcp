---
name: git-commit
description: Create a standardized git commit message when the user asks to commit changes, or when amending or rewriting a recent commit. If you're about to run `git commit` on the user's behalf for any reason, route through this skill first.
user-invocable: true
---

# git-commit

Write git commit messages following conventional commits style: capitalized imperative subject, optional body, and a `Co-Authored-By:` trailer crediting the Claude model.

## Workflow

1. Run `git status` and `git diff --staged` in parallel to inspect the current state.
2. If nothing is staged, check unstaged changes and untracked files. Ask the user what to stage — do NOT stage files automatically.
3. Analyze the staged diff to determine type, scope, subject, and body.
4. Build the message and create the commit. For multi-line messages, pass via heredoc to avoid quoting issues:
   ```bash
   git commit -m "$(cat <<'EOF'
   <message here>
   EOF
   )"
   ```
5. Run `git log -1` to confirm the commit message looks correct.

## Commit message format

```
<type>(<scope>): <Subject>

- <Change 1>
- <Change 2>

Co-Authored-By: <Model Name> <noreply@anthropic.com>
```

A blank line must separate the subject from the body, and the body from the trailer.

## Commit types

Pick the type matching the **primary intent** of the change. If ambiguous, choose the type that reflects the goal, not the side effect.

| Type       | When to use                            |
|------------|----------------------------------------|
| `feat`     | New user-facing feature                |
| `fix`      | Bug fix                                |
| `docs`     | Documentation-only changes             |
| `refactor` | Restructuring without behavior change  |
| `test`     | Adding or modifying tests only         |
| `chore`    | Maintenance tasks, build scripts, etc. |

## Scope

Short, lowercase noun for the affected area (`auth`, `api`, `mcp`, `db`). Omit parentheses entirely when the change is cross-cutting or has no natural scope — e.g., `refactor: Rename ambiguous variables for clarity`.

## Writing rules

- **Subject:** Capitalized, imperative present tense ("Add" not "Added"),
~50 chars (hard ceiling ~72), no trailing period. Be specific —
"Fix bug" is useless; "Handle empty input in tokenizer" is useful.
- **Body:** Optional. Wrap at ~72 chars per line. Explain why and provide
context (not just what changed). Use bullet points or short paragraphs.
Skip for truly self-explanatory one-liners.

## Co-Authored-By trailer

Use the human-friendly name of the model running the current session (e.g., `Claude Opus 4.7`, `Claude Sonnet 4.6`, `Claude Haiku 4.5`). If unsure which model, check system context or ask — don't guess at a name that might not exist.

## Safety rules

- NEVER update git config
- NEVER run destructive commands (`--force`, `reset --hard`) without explicit request
- NEVER skip hooks (`--no-verify`, `--no-gpg-sign`) unless user asks
- NEVER force push to main/master
- Do NOT commit files that likely contain secrets (`.env`, credentials, keys) — warn the user instead
- Always create a NEW commit — never amend unless the user explicitly requests it

## Example

```
feat(auth): Add password reset functionality

- Add forgot password form
- Implement email verification flow
- Add password reset endpoint

Co-Authored-By: Claude Opus 4.7 (https://github.com/claude)
```
