package main;

import domain.Database;
import org.springframework.beans.factory.annotation.Autowired;
import org.springframework.stereotype.Component;
import org.springframework.web.client.RestTemplate;
@Component
public class Console {

    private static final String URLDatabase = "http://localhost:8080/api/database";
    private final RestTemplate restTemplate;
    public Console(RestTemplate rt) {
        restTemplate = rt;
    }

    public void runConsole() {
        Database result = restTemplate.postForObject(URLDatabase, "name", Database.class);
        System.out.println(result.getName());
    }
}
