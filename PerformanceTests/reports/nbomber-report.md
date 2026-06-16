> test info



test suite: `nbomber_default_test_suite_name`

test name: `nbomber_default_test_name`

session id: `2026-06-16_20-10-26_1a15c863`

> scenario stats



scenario: `active_visits_100_requests`

  - ok count: `100`

  - fail count: `0`

  - all data: `0.042` MB

  - duration: `00:00:02`

load simulations:

  - `inject`, rate: `50`, interval: `00:00:01`, during: `00:00:02`

|step|ok stats|
|---|---|
|name|`global information`|
|request count|all = `100`, ok = `100`, RPS = `50`|
|latency (ms)|min = `4.34`, mean = `93.68`, max = `400.08`, StdDev = `97.92`|
|latency percentile (ms)|p50 = `68.8`, p75 = `117.25`, p95 = `310.53`, p99 = `371.71`|
|data transfer (KB)|min = `0.427`, mean = `0.427`, max = `0.427`, all = `0.042` MB|


> status codes for scenario: `active_visits_100_requests`



|status code|count|message|
|---|---|---|
|OK|100||


